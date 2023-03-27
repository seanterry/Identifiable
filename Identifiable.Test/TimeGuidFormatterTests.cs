// SPDX-License-Identifier: MIT
// Copyright (c) Sean Terry (seanterry@outlook.com) and Contributors
// This file is licensed to you under the MIT license.

using System.Diagnostics.CodeAnalysis;

namespace Identifiable.Test;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public abstract partial class TimeGuidFormatterTests
{
    protected abstract TimeGuid.IFormatter instance();

    long time;
    short clock;
    byte[] node = new byte[6];
    Guid method() => instance().Format( time, clock, node );

    [Fact]
    public void Requires_node()
    {
        node = null!;
        Assert.Throws<ArgumentNullException>( nameof(node), () => method() );
    }

    [Theory]
    [InlineData( 5 )]
    [InlineData( 7 )]
    public void Requires_node_length_of_6( int length )
    {
        node = new byte[length];
        Assert.Throws<ArgumentException>( nameof(node), () => method() );
    }
}
