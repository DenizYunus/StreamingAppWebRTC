using System.IO;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class CertificateUpdater : MonoBehaviour
{
    void Start()
    {
        UpdateCACertificates();
    }

    private void UpdateCACertificates()
    {
        TextAsset cacertFile = Resources.Load<TextAsset>("cacert"); // Remove the file extension

        if (cacertFile == null)
        {
            Debug.LogError("Failed to load cacert.pem file.");
            return;
        }

        var certificates = new X509Certificate2Collection();
        certificates.Import(cacertFile.bytes); // Convert TextAsset to byte array directly

        System.Net.ServicePointManager.ServerCertificateValidationCallback = (message, cert, chain, sslPolicyErrors) =>
        {
            foreach (X509Certificate2 ca in certificates)
            {
                chain.ChainPolicy.ExtraStore.Add(ca);
            }

            chain.Build(new X509Certificate2(cert));
            return true;
        };
    }
}
