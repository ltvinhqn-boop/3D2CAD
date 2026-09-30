using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using System.Text;
using System.Management;

public class HardwareInfo
{
    public string GetAppName()
    {
        return "AUTODRAFTING";
    }
    public string GetCPUId()
    {
        string cpuId = string.Empty;
        ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT ProcessorId FROM Win32_Processor");

        foreach (ManagementObject obj in searcher.Get())
        {
            cpuId = obj["ProcessorId"].ToString();
            break;
        }

        return cpuId;
    }

    public string GetHDDSerialNumbers()
    {
        List<string> hddSerials = new List<string>();
        ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_DiskDrive");

        foreach (ManagementObject obj in searcher.Get())
        {
            string hddSerial = obj["SerialNumber"].ToString();
            hddSerials.Add(hddSerial);
        }

        return string.Join("_", hddSerials);
    }


    public string GetBIOSVersion()
    {
        string biosVersion = string.Empty;
        ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_BIOS");

        foreach (ManagementObject obj in searcher.Get())
        {
            biosVersion = obj["Version"].ToString();
            break;
        }

        return biosVersion;
    }

    public string GenerateProductID()
    {
        string appName = GetAppName();
        string cpuId = GetCPUId();
        string hddSerial = "11111";//GetHDDSerialNumbers();
        string biosVersion = GetBIOSVersion();

        string combinedInfo = $"{appName}-{cpuId}-{hddSerial}-{biosVersion}";

        // Hash the combined information
        string hashedInfo = CalculateMD5Hash(combinedInfo);

        // Format the product ID
        string formattedProductID = FormatProductID(hashedInfo);

        return formattedProductID;
    }
    public string GenerateKey(string key)
    {                 

        // Hash the combined information
        string hashedInfo = CalculateMD5Hash(key);

        // Format the product ID
        string formattedProductID = FormatProductID(hashedInfo);

        return formattedProductID;
    }

    private string CalculateMD5Hash(string input)
    {
        using (MD5 md5 = MD5.Create())
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();
            foreach (byte b in hashBytes)
            {
                sb.Append(b.ToString("X2"));
            }
            return sb.ToString();
        }
    }
    public string ConvertStringToSecureCode(string Input1)
    {
        MD5 Secu1 = MD5.Create();
        byte[] data1 = Secu1.ComputeHash(Encoding.Default.GetBytes(Input1));
        StringBuilder sbd = new StringBuilder();
        for (int i = 0; i <= data1.Length - 1; i++)
        {
            sbd.Append(data1[i].ToString("x2"));
        }
        return sbd.ToString();
    }
    private string FormatProductID(string input)
    {
        string formatted = input.Substring(0, 5) + "-" + input.Substring(5, 5) + "-" + input.Substring(10, 5) + "-" + input.Substring(15, 5) + "-" + input.Substring(20, 5);
        return formatted.ToUpper();
    }



    


    public string DecryptLicenseKey(string encryptedLicenseKey, byte[] key, byte[] iv)
    {
        using (var tripledes = new TripleDESCryptoServiceProvider())
        {
            tripledes.Key = key;
            tripledes.IV = iv;
            tripledes.Mode = CipherMode.CBC;
            tripledes.Padding = PaddingMode.PKCS7;

            byte[] data = Convert.FromBase64String(encryptedLicenseKey);
            byte[] decryptedData = tripledes.CreateDecryptor(tripledes.Key, tripledes.IV).TransformFinalBlock(data, 0, data.Length);

            return Encoding.UTF8.GetString(decryptedData);
        }
    }
    //public string EncodeString(string input)
    //{
    //    // Tạo một StringBuilder để xây dựng chuỗi đầu ra
    //    StringBuilder sb = new StringBuilder();

    //    // Chia chuỗi đầu vào thành các phần có độ dài 5 ký tự
    //    for (int i = 0; i < input.Length; i += 5)
    //    {
    //        // Lấy 5 ký tự tiếp theo và thêm vào chuỗi đầu ra
    //        string part = input.Substring(i, Math.Min(5, input.Length - i));
    //        sb.Append(part.PadRight(5, 'X'));

    //        // Thêm dấu gạch ngang nếu đây không phải là phần cuối cùng
    //        if (i + 5 < input.Length)
    //        {
    //            sb.Append("-");
    //        }
    //    }

    //    return sb.ToString();
    //}
    //public string DecodeString(string encoded)
    //{
    //    // Tách chuỗi đã mã hóa thành các phần
    //    string[] parts = encoded.Split('-');

    //    // Xây dựng chuỗi đầu ra bằng cách loại bỏ các ký tự 'X'
    //    StringBuilder sb = new StringBuilder();
    //    foreach (string part in parts)
    //    {
    //        sb.Append(part.TrimEnd('X'));
    //    }

    //    return sb.ToString();
    //}
    //public string GetEncryptionKey(string shortKey)
    //{
    //    using (var sha256 = SHA256.Create())
    //    {
    //        byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(shortKey));
    //        byte[] encryptionKey = new byte[24];
    //        Array.Copy(hash, 0, encryptionKey, 0, 24);

    //        // Chuyển đổi byte[] thành chuỗi Hex
    //        return BitConverter.ToString(encryptionKey).Replace("-", "");
    //    }
    //}

}

