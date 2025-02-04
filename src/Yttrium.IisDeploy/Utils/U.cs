/*
 * Assembly Microsoft.Web.Administration, Version=10.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
 */
namespace Yttrium.IisDeploy;

/// <summary />
internal class U
{
    private static readonly char[] _hexValues = new char[ 16 ]
    {
        '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
        'A', 'B', 'C', 'D', 'E', 'F'
    };


    /// <summary />
    public static string ConvertBytesToCertificateHexString( byte[] sArray )
    {
        uint start = 0;
        uint end = (uint) sArray.Length;

        string result = "";

        if ( sArray != null )
        {
            char[] array = new char[ ( end - start ) * 2 ];
            uint num = start;
            uint num2 = 0u;
            for ( ; num < end; num++ )
            {
                uint num3 = (uint) ( ( sArray[ num ] & 0xF0 ) >> 4 );
                array[ num2++ ] = _hexValues[ num3 ];
                num3 = sArray[ num ] & 0xFu;
                array[ num2++ ] = _hexValues[ num3 ];
            }

            result = new string( array );
        }

        return result;
    }


    /// <summary />
    public static byte[] ConvertCertificateHexStringToBytes( string hexString )
    {
        if ( hexString == null )
        {
            throw new ArgumentNullException( "hexString" );
        }

        bool flag = false;
        int num = 0;
        int num2 = hexString.Length;
        if ( num2 >= 2 && hexString[ 0 ] == '0' && ( hexString[ 1 ] == 'x' || hexString[ 1 ] == 'X' ) )
        {
            num2 = hexString.Length - 2;
            num = 2;
        }

        if ( num2 % 2 != 0 )
        {
            _ = num2 % 3;
            _ = 2;
        }

        byte[] array;
        if ( num2 >= 3 && hexString[ num + 2 ] == ' ' )
        {
            flag = true;
            array = new byte[ num2 / 3 + 1 ];
        }
        else
        {
            array = new byte[ num2 / 2 ];
        }

        int num3 = 0;
        while ( num < hexString.Length )
        {
            int num4 = ConvertHexDigit( hexString[ num ] );
            int num5 = ConvertHexDigit( hexString[ num + 1 ] );
            array[ num3 ] = (byte) ( num5 | ( num4 << 4 ) );
            if ( flag )
            {
                num++;
            }

            num += 2;
            num3++;
        }

        return array;
    }


    public static int ConvertHexDigit( char val )
    {
        if ( val <= '9' && val >= '0' )
        {
            return val - 48;
        }

        if ( val >= 'a' && val <= 'f' )
        {
            return val - 97 + 10;
        }

        if ( val >= 'A' && val <= 'F' )
        {
            return val - 65 + 10;
        }

        return 0;
    }
}
