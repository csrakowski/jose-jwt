﻿using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Jose.keys;

namespace Jose
{
    public class RsaOaep256KeyManagement : IKeyManagement
    {
        public byte[][] WrapNewKey(int cekSizeBits, object key, IDictionary<string, object> header)
        {
            var cek = Arrays.Random(cekSizeBits);

            return new byte[][] { cek, this.WrapKey(cek, key, header) };
        }
        
        public byte[] WrapKey(byte[] cek, object key, IDictionary<string, object> header)
        {            
        #if NET40
            if (key is CngKey)
            {
                var publicKey = (CngKey)key;

                return RsaOaep.Encrypt(cek, publicKey, CngAlgorithm.Sha256);
            }

            if (key is RSACryptoServiceProvider)
            {
                //This is for backward compatibility only with 2.x 
                //To be removed in 3.x 
                var publicKey = RsaKey.New(((RSACryptoServiceProvider)key).ExportParameters(false));

                return RsaOaep.Encrypt(cek, publicKey, CngAlgorithm.Sha256);
            }

            throw new ArgumentException("RsaKeyManagement algorithm expects key to be of CngKey or RSACryptoServiceProvider types.");

        #elif NET461
            if (key is CngKey)
            {
                var publicKey = (CngKey) key;

                return RsaOaep.Encrypt(cek, publicKey, CngAlgorithm.Sha256);
            }

            if (key is RSACryptoServiceProvider)
            {
                //This is for backward compatibility only with 2.x 
                //To be removed in 3.x 
                var publicKey = RsaKey.New(((RSACryptoServiceProvider) key).ExportParameters(false));

                return RsaOaep.Encrypt(cek, publicKey, CngAlgorithm.Sha256);
            }

            if (key is RSA)
            {
	            var publicKey = (RSA) key;

                return publicKey.Encrypt(cek, RSAEncryptionPadding.OaepSHA256);
            }

            throw new ArgumentException("RsaKeyManagement algorithm expects key to be of either CngKey or RSA types.");


#elif NETSTANDARD
            var publicKey = Ensure.Type<RSA>(key, "RsaKeyManagement algorithm expects key to be of RSA type.");

            return publicKey.Encrypt(cek, RSAEncryptionPadding.OaepSHA256);
#endif

        }

        public byte[] Unwrap(byte[] encryptedCek, object key, int cekSizeBits, IDictionary<string, object> header)
        {
        #if NET40
            if(key is RSACryptoServiceProvider)
            {
                //This is for backward compatibility only with 2.x 
                //To be removed in 3.x 
                var privateKey = RsaKey.New(((RSACryptoServiceProvider) key).ExportParameters(true));

                return RsaOaep.Decrypt(encryptedCek, privateKey, CngAlgorithm.Sha256);

            }

            if(key is CngKey)
            {
                var privateKey = (CngKey) key;

	            return RsaOaep.Decrypt(encryptedCek, privateKey, CngAlgorithm.Sha256);
            }

            throw new ArgumentException("RsaKeyManagement algorithm expects key to be of CngKey type.");

         #elif NET461
            if (key is CngKey)
            {
                var privateKey = (CngKey) key;

	            return RsaOaep.Decrypt(encryptedCek, privateKey, CngAlgorithm.Sha256);
            }

            if (key is RSACryptoServiceProvider)
            {
                //This is for backward compatibility only with 2.x 
                //To be removed in 3.x 
                var privateKey = RsaKey.New(((RSACryptoServiceProvider) key).ExportParameters(true));

                return RsaOaep.Decrypt(encryptedCek, privateKey, CngAlgorithm.Sha256);
            }

            if (key is RSA)
            {
                var privateKey = (RSA) key;

                return privateKey.Decrypt(encryptedCek, RSAEncryptionPadding.OaepSHA256);				
            }

            throw new ArgumentException("RsaKeyManagement algorithm expects key to be of either CngKey or RSA types.");

        #elif NETSTANDARD
            var privateKey = Ensure.Type<RSA>(key, "RsaKeyManagement algorithm expects key to be of RSA type.");

            return privateKey.Decrypt(encryptedCek, RSAEncryptionPadding.OaepSHA256);
        #endif
        }
    }
}