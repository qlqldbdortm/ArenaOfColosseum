using System;
using System.Threading.Tasks;
using Colosseum.Network;
using UnityEngine;

namespace Colosseum.Utility
{
    public static class StringChecker
    {
        public static async Task HasFineNicknameAsync(string nickname, Action<string, string> callback = null)
        {
            string errorMessage = null;
            try
            {
                foreach (var checker in Checkers)
                {
                    if (!string.IsNullOrWhiteSpace(errorMessage = await checker(nickname))) break;
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
            finally
            {
                callback?.Invoke(nickname, errorMessage);
            }
        }



        private static readonly Func<string, Task<string>>[] Checkers =
        {
            nickname =>
            {
                bool hasError = string.IsNullOrWhiteSpace(nickname);
                if (hasError)
                {
                    return Task.FromResult("공백은 닉네임으로 사용할 수 없습니다.");
                }
                return Task.FromResult(string.Empty);
            },
            nickname =>
            {
                bool hasError = nickname.Trim().Length is < 2 or > 10;
                if (hasError)
                {
                    return Task.FromResult("닉네임은 2자 이상, 10자 이하로 만들어야 합니다.");
                }
                return Task.FromResult(string.Empty);
            },
            /*async nickname =>
            {
                bool hasError = await AuthManager.Instance.IsNicknameAvailable(nickname);
                if (hasError)
                {
                    return "이미 존재하는 닉네임입니다.";
                }
                return null;
            },*/
        };
    }
}