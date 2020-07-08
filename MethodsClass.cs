﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;

namespace StockTracker
{
    class MethodsClass
    {
        public static string CreateKey(string eMail)
        {
            string newInput = eMail + "raxacoricofallapatorius";
            string sha512 = CryptoClass.EncodeSHA512(newInput);
            string hash = CryptoClass.EncodeMd5(sha512);
            return hash;
        }

        public static bool LicenseCheck(string Key, string License)
        {
            if(Key == License)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    class Regedit
    {
        public static string[] Read()
        {
            string[] LicanseInfo = new string[2];
            //opening the subkey  
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\RK\StockTracker");

            //if it does exist, retrieve the stored values  
            if (key != null)
            {
                LicanseInfo[0] = key.GetValue("EMail").ToString();
                LicanseInfo[1] = key.GetValue("LicenseKey").ToString();
                key.Close();
                return LicanseInfo;
            }
            else
            {
                return null;
            }
        }

        public static void Write(string EMail, string LicenseKey)
        {
            //accessing the CurrentUser root element  
            //and adding "OurSettings" subkey to the "SOFTWARE" subkey  
            RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\RK\StockTracker");

            //storing the values  
            key.SetValue("EMail", EMail);
            key.SetValue("LicenseKey", LicenseKey);
            key.Close();
        }
    }

    class CryptoClass
    {
        public static string EncodeMd5(string rawInput)
        {
            // Use input string to calculate MD5 hash
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(rawInput);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

        public static string EncodeSHA512(string rawInput)
        {
            // Use input string to calculate MD5 hash
            using (SHA512 sha512 = SHA512.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(rawInput);
                byte[] hashBytes = sha512.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
    }

}
