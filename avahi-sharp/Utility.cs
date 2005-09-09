/* $Id$ */

/***
  This file is part of avahi.

  avahi is free software; you can redistribute it and/or modify it
  under the terms of the GNU Lesser General Public License as
  published by the Free Software Foundation; either version 2.1 of the
  License, or (at your option) any later version.

  avahi is distributed in the hope that it will be useful, but WITHOUT
  ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
  or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General
  Public License for more details.

  You should have received a copy of the GNU Lesser General Public
  License along with avahi; if not, write to the Free Software
  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307
  USA.
***/

using System;
using System.Net;
using System.Text;
using System.Runtime.InteropServices;
using Mono.Unix;


namespace Avahi
{
    internal class Utility
    {
        [DllImport ("libc")]
        private static extern int strlen (IntPtr ptr);

        [DllImport ("avahi-common")]
        private static extern IntPtr avahi_address_snprint (IntPtr buf, int size, IntPtr address);

        public static string PtrToString (IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
                return null;
            
            int len = strlen (ptr);
            byte[] bytes = new byte[len];
            Marshal.Copy (ptr, bytes, 0, len);
            return Encoding.UTF8.GetString (bytes);
        }

        public static string PtrToStringFree (IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
                return null;
            
            string ret = PtrToString (ptr);
            Free (ptr);
            return ret;
        }

        public static IntPtr StringToPtr (string str)
        {
            if (str == null)
                return IntPtr.Zero;

            byte[] bytes = Encoding.UTF8.GetBytes (str);
            IntPtr buf = Stdlib.malloc ((uint) bytes.Length + 1);
            Marshal.Copy (bytes, 0, buf, bytes.Length);
            Marshal.WriteByte (buf, bytes.Length, 0);
            return buf;
        }

        public static void Free (IntPtr ptr)
        {
            Stdlib.free (ptr);
        }

        public static IPAddress PtrToAddress (IntPtr ptr)
        {
            IPAddress address = null;
            
            if (ptr != IntPtr.Zero) {
                IntPtr buf = Stdlib.malloc (256);
                IntPtr addrPtr = avahi_address_snprint (buf, 256, ptr);
                address = IPAddress.Parse (Utility.PtrToString (addrPtr));
                Utility.Free (addrPtr);
            }

            return address;
        }
    }
}