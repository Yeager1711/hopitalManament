using BELibrary.Core.Entity.Repositories;
using BELibrary.Core.Utils;
using BELibrary.DbContext;
using BELibrary.Entity;
using BELibrary.Utils;
using System;
using System.Linq;

namespace BELibrary.Persistence.Repositories
{
    public class AccountRepository : Repository<Account>, IAccountRepository
    {
        public AccountRepository(HospitalManagementDbContext context)
            : base(context)
        {
        }

        public Account ValidBEAccount(string username, string password)
        {
            // Kiểm tra input không hợp lệ
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return null;
            }

            // Mã hóa mật khẩu
            var passwordFactory = password + VariableExtensions.KeyCrypto;
            var passwordCrypto = CryptorEngine.Encrypt(passwordFactory, true);

            // Truy vấn dữ liệu
            var account = HospitalManagementDbContext.Accounts.FirstOrDefault(x =>
                (x.Role == RoleKey.Admin || x.Role == RoleKey.Doctor) &&
                x.UserName.ToLower() == username.ToLower() &&
                x.Password == passwordCrypto);

            return account;
        }

        public Account GetAccountByUsername(string username)
        {
            // Kiểm tra input không hợp lệ
            if (string.IsNullOrWhiteSpace(username))
            {
                return null;
            }

            // Truy vấn dữ liệu
            var account = HospitalManagementDbContext.Accounts.FirstOrDefault(x =>
                (x.Role == RoleKey.Admin || x.Role == RoleKey.Doctor) &&
                x.UserName.ToLower() == username.ToLower());

            return account;
        }

        public Account GetAccountFeByUsername(string username)
        {
            // Kiểm tra input không hợp lệ
            if (string.IsNullOrWhiteSpace(username))
            {
                return null;
            }

            // Truy vấn dữ liệu
            var account = HospitalManagementDbContext.Accounts.FirstOrDefault(x =>
                x.Role == RoleKey.Patient &&
                x.UserName.ToLower() == username.ToLower());

            return account;
        }

        public Account ValidFeAccount(string username, string password)
        {
            // Kiểm tra input không hợp lệ
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return null;
            }

            // Mã hóa mật khẩu
            var passwordFactory = password + VariableExtensions.KeyCryptorClient;
            var passwordCrypto = CryptorEngine.Encrypt(passwordFactory, true);

            // Truy vấn dữ liệu
            var account = HospitalManagementDbContext.Accounts.FirstOrDefault(x =>
                x.UserName.ToLower() == username.ToLower() &&
                x.Password == passwordCrypto);

            return account;
        }

        public HospitalManagementDbContext HospitalManagementDbContext => Context;
    }
}
