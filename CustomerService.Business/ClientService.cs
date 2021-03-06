﻿using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using CustomerService.Business.Models;
using CustomerService.Repositories.Interfaces;

namespace CustomerService.Business
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;
        private readonly IGoogleAuthService _googleAuthService;
        private readonly IEmailService _emailService;

        public ClientService(IClientRepository ClientRepository, IEmailService emailService, IGoogleAuthService googleAuthService)
        {
            _clientRepository = ClientRepository;
            _emailService = emailService;
            _googleAuthService = googleAuthService;
        }

        public Client GetClient(Guid id)
        {
            var client = _clientRepository.GetClient(id);
            if (client == null)
            {
                return null;
            }

            return new Client(client);
        }

        public Client Authentification(string name, string password)
        {
            var client = _clientRepository.GetClientByName(name);
            if (client == null || !client.IsActive)
            {
                return null;
            }
            if (client.PasswordHash != BuildPasswordHash(password))
            {
                return null;
            }

            return new Client(client);
        }

        public bool CheckNameAvailability(string name)
        {
            var client = _clientRepository.GetClientByName(name);
            return (client == null) ? true : false;
        }

        public bool CheckEmailAvailability(string email)
        {
            var client = _clientRepository.GetClientByEmail(email);
            return (client == null) ? true : false;
        }

        
        public Client CreateClient(string email, string name, string password)
        {
            var activationCode = GenerateCode(24);
            var hashedPassword = BuildPasswordHash(password);
            var createdClient = _clientRepository.CreateClient(email, name, hashedPassword, activationCode);
            if (createdClient == null)
            {
                return null;
            }

            var letter = CreateRegistrationEmail(activationCode);
            _emailService.SendEmail(email, "Regisration confirm letter", letter);

            return new Client(createdClient);
        }

        public bool ActivateClient(string activateCode)
        {
            return _clientRepository.ActivateClient(activateCode);
        }

        public Client UpdateClient(Guid id, string email, string ClientName)
        {
            var updatedClient = _clientRepository.UpdateClient(id, email, ClientName);
            return updatedClient == null ? null : new Client(updatedClient);
        }

        public bool SendActivationCode(string email)
        {
            var client = _clientRepository.GetClientByEmail(email);

            if (client == null)
            {
                return false;
            }

            var letter = CreateRegistrationEmail(client.ActivationCode);
            _emailService.SendEmail(email, "Regisration confirm letter", letter);

            return true;
        }


        public bool ValidateClientByGoogleAuth(Guid id, string oneTimePassword)
        {
            var client = _clientRepository.GetClient(id);
            if (client == null)
            {
                return false;
            }
            if ((!client.GoogleAuthActive.HasValue || !client.GoogleAuthActive.Value) && client.GoogleAuthCode == null)
            {
                return true;
            }

            return _googleAuthService.Validate(client.GoogleAuthCode, oneTimePassword);
        }

        public GoogleAuthCode CreateGoogleAuthCode(Guid id)
        {
            var client = _clientRepository.GetClient(id);
            if (client == null)
            {
                return null;
            }

            if (!string.IsNullOrEmpty(client.GoogleAuthCode) && client.GoogleAuthActive == true)
            {
                throw new Exception("Client already has active GoogleAuthCode.");
            }

            var authCode = GenerateCode(26);
            var code = _googleAuthService.Generate(client.Email, authCode);

            if (code != null)
            {
                _clientRepository.UpdateGoogleAuthCode(id, authCode);
            }

            return code;
        }

        public bool? SetGoogleAuthCode(Guid id, string oneTimePassword)
        {
            var client = _clientRepository.GetClient(id);
            if (client == null)
            {
                return null;
            }
            if (client.GoogleAuthActive == true)
            {
                return false;
            }

            bool isCorrectPIN = _googleAuthService.Validate(client.GoogleAuthCode, oneTimePassword);
            if (!isCorrectPIN)
            {
                throw new Exception("Password invalid.");
            }

            return _clientRepository.ActivateGoogleAuthCode(id);
        }

        public bool? DeactivateGoogleAuthCode(Guid id, string oneTimePassword)
        {
            var client = _clientRepository.GetClient(id);
            if (client == null)
            {
                return null;
            }
            if (!client.GoogleAuthActive.HasValue || client.GoogleAuthActive == false)
            {
                return false;
            }

            bool isCorrectPIN = _googleAuthService.Validate(client.GoogleAuthCode, oneTimePassword);
            if (!isCorrectPIN)
            {
                throw new Exception("Password invalid.");
            }

            return _clientRepository.DeactivateGoogleAuthCode(id);
        }

        private string CreateRegistrationEmail(string activationCode)
        {
            string body = string.Empty;

            using (StreamReader reader = new StreamReader("EmailTemplates/RegistrationClientEmail.html"))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("\r\n", "").Replace("{code}", activationCode);

            return body;
        }

        private string GenerateCode(int len)
        {
            var result = string.Empty;
            var alphanum = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz".ToCharArray();

            for (var i = 0; i < len; ++i)
            {
                result += alphanum[(new Random().Next(0, (alphanum.Length - 1)))];
            }

            return result;
        }

        private static string BuildPasswordHash(string password)
        {
            byte[] Salt = { 0xf0, 0x87, 0x33, 0x50, 0x07, 0x91, 0x12, 0x61, 0x07, 0x03, 0x21, 0x57, 0x22, 0x02, 0x01, 0xA4 };
            byte[] passwordHash;

            var passwordBytes = Encoding.UTF8.GetBytes(password);

            using (var sha256 = new SHA256Managed())
            {
                var message = new byte[Salt.Length + passwordBytes.Length];

                Array.Copy(Salt, message, Salt.Length);
                Array.Copy(passwordBytes, 0, message, Salt.Length, passwordBytes.Length);

                passwordHash = sha256.ComputeHash(message);
            }

            return Convert.ToBase64String(passwordHash);
        }
    }
}