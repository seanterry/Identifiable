// SPDX-License-Identifier: MIT
// Copyright (c) Sean Terry (seanterry@outlook.com) and Contributors
// This file is licensed to you under the MIT license.

using System.Security.Cryptography;
using System.Text;

namespace Identifiable;

/// <summary>
/// Creates GUIDs based on a namespace and name.
/// </summary>
public static class NamedGuid
{
    /// <summary>
    /// Computes and returns a name-based GUID using the specified algorithm as defined in RFC 4122.
    /// https://tools.ietf.org/html/rfc4122#section-4.3.
    /// </summary>
    /// <param name="algorithm">Hash algorithm to use for generating the name. SHA-1 is recommended.</param>
    /// <param name="namespace">Namespace identifier.</param>
    /// <param name="name">Name for which to create a GUID.</param>
    public static Guid Compute( NamedGuidAlgorithm algorithm, Guid @namespace, string name ) =>
        ComputeInternal( algorithm, @namespace, name );

    /// <summary>
    /// Computes and returns a name-based GUID using the specified algorithm as defined in RFC 4122.
    /// https://tools.ietf.org/html/rfc4122#section-4.3.
    /// </summary>
    /// <param name="algorithm">Hash algorithm to use for generating the name. SHA-1 is recommended.</param>
    /// <param name="namespace">Namespace identifier.</param>
    /// <param name="name">Name for which to create a GUID.</param>
    public static Guid Compute( Guid @namespace, string name, NamedGuidAlgorithm algorithm = NamedGuidAlgorithm.SHA1 ) =>
        ComputeInternal( algorithm, @namespace, name );

    /// <summary>
    /// Corrects endianness of the GUID byte order.
    /// The first DWORD and two following WORDs may be in little-endian order.
    /// The last 8 bytes are already in big-endian order.
    /// </summary>
    /// <param name="bytes">GUID bytes.</param>
    /// <remarks>>
    /// Learned from documentation formerly at:
    /// https://msdn.microsoft.com/en-us/library/windows/desktop/aa373931(v=vs.85).aspx
    /// </remarks>
    static void CorrectEndianness( byte[] bytes )
    {
        if ( !BitConverter.IsLittleEndian ) return;
        Array.Reverse( bytes, 0, 4 );
        Array.Reverse( bytes, 4, 2 );
        Array.Reverse( bytes, 6, 2 );
    }

    /// <summary>
    /// Returns the implementation of the specified hash algorithm.
    /// </summary>
    /// <param name="algorithm">Hash algorithm to create.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">The algorithm is unknown.</exception>
    static HashAlgorithm CreateHashAlgorithm( NamedGuidAlgorithm algorithm )
    {
        return algorithm switch
        {
            NamedGuidAlgorithm.MD5 => MD5.Create(),
            NamedGuidAlgorithm.SHA1 => SHA1.Create(),
            _ => throw new ArgumentException( $"Unknown algorithm: {algorithm}", nameof( algorithm ) )
        };
    }

    /// <summary>
    /// Returns the version field value for the specified algorithm.
    /// </summary>
    /// <param name="algorithm">Algorithm whose version to return.</param>
    /// <exception cref="ArgumentException">The algorithm is unknown.</exception>
    static byte GetVersion( NamedGuidAlgorithm algorithm ) => algorithm switch
    {
        NamedGuidAlgorithm.MD5 => 0x30,
        NamedGuidAlgorithm.SHA1 => 0x50,
        _ => throw new ArgumentException( $"Unknown algorithm: {algorithm}", nameof( algorithm ) )
    };

    /// <summary>
    /// Generates the hash to use for the name-based GUID.
    /// </summary>
    /// <param name="algorithm">Hash algorithm to use.</param>
    /// <param name="namespace">Namespace identifier.</param>
    /// <param name="name">Name for which to create a GUID.</param>
    /// <returns>A byte array containing the hash.</returns>
    static byte[] Hash( NamedGuidAlgorithm algorithm, byte[] @namespace, byte[] name )
    {
        using var hasher = CreateHashAlgorithm( algorithm );
        hasher.TransformBlock( @namespace, 0, @namespace.Length, null, 0 );
        hasher.TransformFinalBlock( name, 0, name.Length );
        return hasher.Hash ?? throw new InvalidOperationException( "Hash algorithm returned null." );
    }

    /// <summary>
    /// Clears the last 4 bits of the version field and sets it to the appropriate value.
    /// </summary>
    /// <param name="hash">Output of the hash algorithm.</param>
    /// <param name="algorithm">Algorithm that was used to generate the hash.</param>
    static void SetVersion( byte[] hash, NamedGuidAlgorithm algorithm )
    {
        var version = GetVersion( algorithm );
        hash[7] &= 0b00001111;
        hash[7] |= version;
    }

    /// <summary>
    /// Sets the variant field to the appropriate value.
    /// Turns on the first bit and turns off the second bit.
    /// </summary>
    /// <param name="hash">Output of the hash algorithm.</param>
    static void SetVariant( byte[] hash )
    {
        hash[8] |= 0b10000000;
        hash[8] &= 0b10111111;
    }

    /// <summary>
    /// Internal implementation.
    /// </summary>
    internal static Guid ComputeInternal( NamedGuidAlgorithm algorithm, Guid @namespace, string name )
    {
        if ( name == null ) throw new ArgumentNullException( nameof(name) );

        var nameBytes = Encoding.UTF8.GetBytes( name );
        var namespaceBytes = @namespace.ToByteArray();
        CorrectEndianness( namespaceBytes );

        var hash = Hash( algorithm, namespaceBytes, nameBytes );
        Array.Resize( ref hash, 16 );
        CorrectEndianness( hash );

        SetVersion( hash, algorithm );
        SetVariant( hash );

        return new( hash );
    }
}
