using System;
using System.IO;
using System.IO.Compression;
using System.Text;

// source : https: //stackoverflow.com/questions/7343465/compression-decompression-string-with-c-sharp

public class Zipper {
  public static void CopyTo (Stream src, Stream dest) {
    byte[] bytes = new byte[4096];

    int cnt;

    while ((cnt = src.Read (bytes, 0, bytes.Length)) != 0) {
      dest.Write (bytes, 0, cnt);
    }
  }

  public static string Zip (string str) {
    var bytes = Encoding.UTF8.GetBytes (str);

    using (var msi = new MemoryStream (bytes))
    using (var mso = new MemoryStream ()) {
      using (var gs = new GZipStream (mso, CompressionMode.Compress)) {
        //msi.CopyTo(gs);
        CopyTo (msi, gs);
      }

      return Convert.ToBase64String (mso.ToArray ());
    }
  }

  public static string Unzip (string value) {
    var bytes = Convert.FromBase64String (value);
    using (var msi = new MemoryStream (bytes))
    using (var mso = new MemoryStream ()) {
      using (var gs = new GZipStream (msi, CompressionMode.Decompress)) {
        //gs.CopyTo(mso);
        CopyTo (gs, mso);
      }

      return Encoding.UTF8.GetString (mso.ToArray ());
    }
  }
}