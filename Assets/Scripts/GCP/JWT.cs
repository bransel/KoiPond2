using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using UnityEngine;

[Serializable]
public class JWT
{
	public string header;
	public string claimset;
	public string signature;
	public string jwt;

	public void GenerateJWT()
	{
		jwt = string.Format("{0}.{1}.{2}", Base64Encode(header), Base64Encode(claimset), signature);
	}

	public string GetJWTHeader()
	{
		string alg = KeyPair("alg", "RS256");
		string typ = KeyPair("typ", "JWT");

		return Bracket(string.Format("{0}, {1}", alg, typ));
	}

	public string GetJWTClaimSet()
	{
		TimeSpan now = DateTime.UtcNow - new DateTime(1970, 1, 1);
		TimeSpan hour = DateTime.UtcNow.AddHours(1) - new DateTime(1970, 1, 1);
		
		string iss = KeyPair("iss", "koi-user@unity-koi.iam.gserviceaccount.com");
		string scope = KeyPair("scope", "https://www.googleapis.com/auth/devstorage.read_write");
		string aud = KeyPair("aud", "https://www.googleapis.com/oauth2/v4/token");
		string exp = KeyPair("exp", Math.Round(hour.TotalSeconds).ToString());
		string iat = KeyPair("iat", Math.Round(now.TotalSeconds).ToString());

		return Bracket(string.Format("{0}, {1}, {2}, {3}, {4}", iss, scope, aud, exp, iat));
	}

	public string GetJWTSignature(byte[] p12)
	{	
		X509Certificate2 cert2 = new X509Certificate2(p12, "notasecret");
		byte[] signature = Encoding.UTF8.GetBytes(Base64Encode(header) + "." + Base64Encode(claimset));
		RSA rsa = cert2.GetRSAPrivateKey();
		return Base64Encode(rsa.SignData(signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1));
	}

	string Quote(string value)
	{
		return string.Format("\"{0}\"", value);
	}

	string KeyPair(string key, string pair)
	{
		return string.Format("{0}:{1}", Quote(key), Quote(pair));
	}

	string Bracket(string value)
	{
		return "{" + value + "}";
	}

	public string Base64Encode(byte[] b)
	{
		string s = Convert.ToBase64String(b);
		s = s.Replace("+", "-");
		s = s.Replace("/", "_");
		s = s.Split("="[0])[0]; // Remove any trailing '='s
		return s;
	}

	public string Base64Encode(string s)
	{
		return Base64Encode(Encoding.UTF8.GetBytes(s));
	}
}