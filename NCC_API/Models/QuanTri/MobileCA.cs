using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.OpenSsl;
using System.IO;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto.Parameters;
using System.Threading.Tasks;
using SimCAService;

namespace Timensit_API.Models
{

    public class MobileCA
    {
        private const string PROCESS_CODE_SIGN = "000001";
        private const string PROCESS_CODE_QUERYCER = "000006";
        private const string MODE_SYNC = "SYNC";
        private const string MODE_ASYNC_SERVER_SERVER = "SS";
        private const string MODE_ASYNC_CLIENT_SERVER = "CS";
        private const string FORMAT_DATE = "{0:yyyyMMddHHmmss}";
        private const string MSSP_ID = "Viettel";
        private const int FORCE_PIN = 1;
        private const int RUN_IMM = 0; // ko yeu cau ky ngay
        private const int SIG_TYPE = 0; // ky van ban
        private const bool FORCE_PIN_SPECIFIED = true;
        private const bool SIG_TYPE_SPECIFIED = true;
        private const bool REQUEST_ID_SPECIFIED = true;

        private static readonly Random getrandom = new Random();
        private static readonly object syncLock = new object();
        public MobileCA()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        public static int GetRandomNumber(int min, int max)
        {
            lock (syncLock)
            { // synchronize
                return getrandom.Next(min, max);
            }
        }
        public long generateRequestId()
        {
            int seq = GetRandomNumber(1, 999999);
            string strTrace = seq.ToString();
            while (strTrace.Length < 6)
            {
                strTrace = "0" + strTrace;
            }
            return long.Parse(DateTime.Now.ToString("yyMMdd") + strTrace);
        }
        public async Task<String[]> GetCertificatefromPrikey(string apId, string msisdn, string certSerial, string prikey)
        {
            try
            {
                //APWsClient ca = new APWsClient();
                //ca.certificateQueryAsync
                APWsClient simCA = new APWsClient();

                //var service=simCA.certificateQueryAsync;

                //APWsClient.transactionInfo transInfo = new MobileCAWS.transactionInfo();
                transactionInfo transInfo = new transactionInfo();

                transInfo.apId = apId;
                transInfo.msspId = MSSP_ID;
                transInfo.processCode = PROCESS_CODE_QUERYCER;
                transInfo.msisdn = msisdn;
                if (certSerial != null)
                {
                    transInfo.certSerial = certSerial;
                }


                transInfo.requestId = generateRequestId();

                transInfo.requestIdSpecified = REQUEST_ID_SPECIFIED;
                transInfo.reqDate = String.Format(FORMAT_DATE, DateTime.Now);


                string strCombine = transInfo.apId + transInfo.msspId
                    + transInfo.processCode + transInfo.requestId.ToString() + transInfo.msisdn;

                //transInfo.mac = createMACMsg(strCombine, fileP12, pass);
                transInfo.mac = RsaSignWithPrivateFromJava(strCombine, prikey);

                transInfo = await simCA.certificateQueryAsync(transInfo);

                //var certParser = new Org.BouncyCastle.X509.X509CertificateParser();
                //Org.BouncyCastle.X509.X509Certificate[] chain = new[] { certParser.ReadCertificate(Convert.FromBase64String(transInfo.certList[0])) };
                return transInfo.certList;
            }
            catch (Exception e)
            {
                //Console.WriteLine("Error: " + e.Message);
                return null;
            }
        }

        public string RsaSignWithPrivateFromJava(string clearText, string privateKey)
        {
            byte[] keyData = Base64.Decode(privateKey);

            AsymmetricKeyParameter privKey = PrivateKeyFactory.CreateKey(keyData);

            RsaPrivateCrtKeyParameters rsaKeyParameters = (RsaPrivateCrtKeyParameters)privKey;
            RSAParameters rsaParameters = new RSAParameters();

            rsaParameters.Modulus = rsaKeyParameters.Modulus.ToByteArrayUnsigned();
            rsaParameters.P = rsaKeyParameters.P.ToByteArrayUnsigned();
            rsaParameters.Q = rsaKeyParameters.Q.ToByteArrayUnsigned();
            rsaParameters.DP = rsaKeyParameters.DP.ToByteArrayUnsigned();
            rsaParameters.DQ = rsaKeyParameters.DQ.ToByteArrayUnsigned();
            rsaParameters.InverseQ = rsaKeyParameters.QInv.ToByteArrayUnsigned();
            rsaParameters.D = rsaKeyParameters.Exponent.ToByteArrayUnsigned();
            rsaParameters.Exponent = rsaKeyParameters.PublicExponent.ToByteArrayUnsigned();


            //rsaParameters.Modulus = rsaKeyParameters.Modulus.ToByteArrayUnsigned();
            //rsaParameters.Exponent = rsaKeyParameters.Exponent.ToByteArrayUnsigned();
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.ImportParameters(rsaParameters);

                // Hash the data
                using (SHA1Managed sha1 = new SHA1Managed())
                {
                    UTF8Encoding encoding = new UTF8Encoding();
                    byte[] dataToByte = encoding.GetBytes(clearText);
                    byte[] hash = sha1.ComputeHash(dataToByte);
                    // Sign the hash
                    return System.Convert.ToBase64String(rsa.SignHash(hash, CryptoConfig.MapNameToOID("SHA1")));
                }

            }
        }
        public async Task<string> signSynchronouswithPrikey(String dataSignByteSHa1, string apId, string msisdn, string dataDisplay, string prikey)
        {
            try
            {
                APWsClient service = new APWsClient();
                transactionInfo transInfo = new transactionInfo();


                transInfo.apId = apId;
                transInfo.msspId = MSSP_ID;
                transInfo.processCode = PROCESS_CODE_SIGN;
                transInfo.msisdn = msisdn;
                //transInfo.msisdn = phone;
                transInfo.msgMode = MODE_SYNC; // co che dong bo
                transInfo.forcePIN = FORCE_PIN; // bat buoc nhap PIN
                transInfo.forcePINSpecified = FORCE_PIN_SPECIFIED;
                transInfo.runImm = RUN_IMM; // ko yeu cau ky ngay
                transInfo.sigType = SIG_TYPE; // ky van ban
                transInfo.sigTypeSpecified = SIG_TYPE_SPECIFIED;
                transInfo.dataDisplay = dataDisplay;
                transInfo.requestIdSpecified = REQUEST_ID_SPECIFIED;
                transInfo.reqDate = String.Format(FORMAT_DATE, DateTime.Now);


                transInfo.requestId = generateRequestId();
                //Console.WriteLine("Thong tin requestID: " + transInfo.requestId);


                //SHA1 sha = new SHA1Managed();
                //byte[] dataSignByteSHa1 = sha.ComputeHash(sh);
                //transInfo.dataSign = Convert.ToBase64String(dataSignByteSHa1);
                transInfo.dataSign = dataSignByteSHa1;


                UTF8Encoding encode = new UTF8Encoding();
                string strCombine = transInfo.apId + transInfo.msspId + transInfo.processCode + transInfo.requestId.ToString() +
                    transInfo.msisdn + transInfo.dataSign + transInfo.dataDisplay + transInfo.sigType.ToString();



                transInfo.mac = RsaSignWithPrivateFromJava(strCombine, prikey);

                //Console.WriteLine("Thong tin MAC : " + transInfo.mac);

                if (transInfo.mac == null)
                {
                    //Console.WriteLine("Error get MAC");
                    return null;
                }

                transInfo = await service.signatureAsync(transInfo);


                //Console.WriteLine("Ket qua tra ve: " + transInfo.errorCode + "|" + transInfo.errorDesc);

                if (transInfo.errorCode != "00")
                {
                    //Console.WriteLine("Error get signature");
                    return null;
                }

                //return Convert.FromBase64String(transInfo.signature);
                return transInfo.signature;
            }
            catch (Exception e)
            {
                //Console.WriteLine("Error: " + e.Message);
                return null;
            }
        }

        public async Task<transactionInfo> Signature(string apId, string msisdn, string prikey)
        {
            APWsClient service = new APWsClient();
            transactionInfo transInfo = new transactionInfo();

            transInfo.apId = apId;
            transInfo.msspId = MSSP_ID;
            transInfo.processCode = PROCESS_CODE_QUERYCER;
            transInfo.msisdn = msisdn;
            transInfo.requestId = generateRequestId();
            transInfo.requestIdSpecified = REQUEST_ID_SPECIFIED;
            transInfo.reqDate = String.Format(FORMAT_DATE, DateTime.Now);
            string strCombine = transInfo.apId + transInfo.msspId
                + transInfo.processCode + transInfo.requestId.ToString() + transInfo.msisdn;
            transInfo.mac = RsaSignWithPrivateFromJava(strCombine, prikey);
            transInfo.msgMode = "SYNC";

            return await service.signatureAsync(transInfo);
        }
    }
}