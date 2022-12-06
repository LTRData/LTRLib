/*
 * LTRLib
 * 
 * Copyright (c) Olof Lagerkvist, LTR Data
 * http://ltr-data.se   https://github.com/LTRData
 */
#if NET30_OR_GREATER || NETCOREAPP

using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LTRLib.Imaging;

using Geodesy.Positions;

/// <summary>
/// Routines for extracting geo tags from images.
/// </summary>
public static class GeoLocators
{
    /// <summary>
    /// Gets photo location coordinates from bitmap metadata
    /// </summary>
    /// <param name="metadata">Bitmap metadata</param>
    /// <returns>WGS84 coordinates</returns>
    /// <exception cref="NotSupportedException">Image metadata does not contain any geo tags</exception>
    public static WGS84Position? GetGeoLocation(this BitmapMetadata? metadata)
    {
        if (metadata is null ||
            metadata.GetQuery("/app1/ifd/gps/{ushort=1}") is not string lat_direction ||
            metadata.GetQuery("/app1/ifd/gps/{ushort=2}") is not ulong[] lat_fields ||
            metadata.GetQuery("/app1/ifd/gps/{ushort=3}") is not string lon_direction ||
            metadata.GetQuery("/app1/ifd/gps/{ushort=4}") is not ulong[] lon_fields)
        {
            throw new NotSupportedException("Image contains no supported metadata");
        }

        return new WGS84Position(
            lat_direction[0], (byte)(lat_fields[0] & 0xFFul), (byte)(lat_fields[1] & 0xFFul), (lat_fields[2] & 0xFFFFul) / 1000d,
            lon_direction[0], (byte)(lon_fields[0] & 0xFFul), (byte)(lon_fields[1] & 0xFFul), (lon_fields[2] & 0xFFFFul) / 1000d);
    }

    /// <summary>
    /// Gets photo location coordinates from bitmap
    /// </summary>
    /// <param name="image">Bitmap image</param>
    /// <returns>WGS84 coordinates</returns>
    /// <exception cref="NotSupportedException">Image metadata does not contain any geo tags</exception>
    public static WGS84Position? GetGeoLocation(this ImageSource image) => (image.Metadata as BitmapMetadata).GetGeoLocation();

    /// <summary>
    /// Gets photo location coordinates from bitmap
    /// </summary>
    /// <param name="image_path">Location of bitmap image</param>
    /// <returns>WGS84 coordinates</returns>
    /// <exception cref="NotSupportedException">Image metadata does not contain any geo tags</exception>
    public static WGS84Position? GetImageGeoLocation(Uri image_path) => BitmapFrame.Create(image_path).GetGeoLocation();

    /// <summary>
    /// Gets photo location coordinates from bitmap
    /// </summary>
    /// <param name="image_path">Location of bitmap image</param>
    /// <returns>WGS84 coordinates</returns>
    /// <exception cref="NotSupportedException">Image metadata does not contain any geo tags</exception>
    public static WGS84Position? GetImageGeoLocation(string image_path) => GetImageGeoLocation(new Uri(image_path));
}

#endif
