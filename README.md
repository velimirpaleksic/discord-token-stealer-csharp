# Discord Token Extraction and Decryption Script
This C# script extracts and decrypts tokens from various applications and browsers by parsing their local storage files. It demonstrates methods used to identify, decrypt, and analyze stored tokens. This project is designed for **educational purposes only** to highlight the importance of secure token storage.

## **Requirements**
- .NET Framework
- Newtonsoft.Json (for JSON parsing)
- Org.BouncyCastle (for AES decryption)

## **Installation**
1. Ensure you have .NET SDK installed on your system.
2. Add the required libraries via NuGet:
   ```shell
   dotnet add package Newtonsoft.Json
   dotnet add package BouncyCastle
   ```

## **How It Works**
1. **Define Paths:**  
   The script defines paths to various applications and browsers where token data might be stored.
2. **Extract Key:**  
   It reads the `Local State` file to obtain the encryption key used for decrypting tokens.
3. **Parse Files:**  
   Searches through LevelDB files and extracts potential tokens using regex patterns.
4. **Decrypt Tokens:**  
   Extracted tokens are decrypted using AES encryption with the obtained key.
5. **Match Tokens:**  
   Tokens are matched against predefined regex patterns to identify valid tokens.
6. **Output:**  
   The script prints out the extracted and decrypted tokens.

## **Files Used**
- **Local State File:**  
  Obtained from application directories to extract the AES encryption key.
- **LevelDB Files:**  
  Found in application storage folders to extract encrypted tokens.

## **Regular Expressions**
- **Basic Regex:** Matches standard token formats (`[\w-]{24}\.[\w-]{6}\.[\w-]{27}`).  
- **New Regex:** Matches MFA token formats (`mfa\.[\w-]{84}`).  
- **Encrypted Regex:** Matches encrypted tokens (`dQw4w9WgXcQ:...`).  

## **Usage**
1. Clone the repository or download the script.
2. Install the required libraries using NuGet.
3. Build and run the script:
   ```shell
   dotnet build
   dotnet run
   ```

## **Disclaimer**
- This script is intended for **educational purposes only**. Use responsibly and ethically.  

## **License**
This script is provided under the [MIT License](LICENSE).  
By using this script, you agree to comply with all applicable laws and regulations and use it only for lawful, ethical purposes.

## **Contact** ✉
- E-mail: [velimir.paleksic@gmail.com](velimir.paleksic@gmail.com).
- VexSystems Github: [github.com/vexsystems](https://github.com/vexsystems).
- VexSystems Instagram: [@vex.systems](https://www.instagram.com/vex.systems/).