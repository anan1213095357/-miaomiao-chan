using LLM.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;


//这个代码是动态创建的脚本  功能就是 保存游戏账号 和按大区搜索有那些账号
namespace 英雄联盟账号助手
{
    public class GameAccount
    {
        public string Account { get; set; }
        public string PasswordHash { get; set; } // 存储密码哈希
        public string Region { get; set; }
        public string SummonerName { get; set; }
    }

    public class 英雄联盟账号助手类
    {
        private readonly List<GameAccount> _gameAccounts;
        private readonly string _dataFilePath = "gameAccounts.json";
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        public 英雄联盟账号助手类()
        {
            _gameAccounts = LoadGameAccounts();
        }

        /// <summary>
        /// 记录游戏账号信息
        /// </summary>
        /// <param name="account">游戏账号</param>
        /// <param name="password">游戏密码</param>
        /// <param name="region">游戏大区</param>
        /// <param name="summonerName">召唤师名字</param>
        /// <returns>记录结果</returns>
        [FunctionDescription("recordGameAccount", "记录游戏账号信息")]
        public string RecordGameAccount(
            [ParameterDescription("游戏账号")] string account,
            [ParameterDescription("游戏密码")] string password,
            [ParameterDescription("游戏大区")] string region,
            [ParameterDescription("召唤师名字")] string summonerName)
        {
            if (string.IsNullOrWhiteSpace(account) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(region) ||
                string.IsNullOrWhiteSpace(summonerName))
            {
                return "所有字段都是必填的。";
            }

            _lock.EnterWriteLock();
            try
            {
                // 检查是否已存在相同账号
                if (_gameAccounts.Any(a => a.Account.Equals(account, StringComparison.OrdinalIgnoreCase)))
                {
                    return $"账号 '{account}' 已经存在。";
                }

                // 生成密码哈希
                string passwordHash = ComputeSha256Hash(password);

                // 创建新的 GameAccount 实例
                var newAccount = new GameAccount
                {
                    Account = account,
                    PasswordHash = passwordHash,
                    Region = region,
                    SummonerName = summonerName
                };

                _gameAccounts.Add(newAccount);
                SaveGameAccounts();

                return $"游戏账号已记录：账号={account}, 大区={region}, 召唤师名字={summonerName}";
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        /// <summary>
        /// 通过游戏大区获取游戏账号列表
        /// </summary>
        /// <param name="region">游戏大区名称</param>
        /// <returns>匹配的游戏账号列表</returns>
        [FunctionDescription("getGameAccountsByRegion", "通过游戏大区获取游戏账号列表")]
        public string GetGameAccountsByRegion(
            [ParameterDescription("游戏大区名称")] string region)
        {
            if (string.IsNullOrWhiteSpace(region))
            {
                return "请提供游戏大区名称以获取游戏账号列表。";
            }

            _lock.EnterReadLock();
            try
            {
                var matchingAccounts = _gameAccounts
                    .Where(a => a.Region.Equals(region, StringComparison.OrdinalIgnoreCase))
                    .Select(a => $"账号: {a.Account}, 召唤师名字: {a.SummonerName}")
                    .ToList();

                if (matchingAccounts.Any())
                {
                    return string.Join("\r\n", matchingAccounts);
                }
                else
                {
                    return $"未找到大区 '{region}' 的任何游戏账号。";
                }
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        /// <summary>
        /// 加载游戏账号信息从 JSON 文件
        /// </summary>
        /// <returns>游戏账号列表</returns>
        private List<GameAccount> LoadGameAccounts()
        {
            if (!File.Exists(_dataFilePath))
            {
                return new List<GameAccount>();
            }

            try
            {
                string json = File.ReadAllText(_dataFilePath);
                return JsonSerializer.Deserialize<List<GameAccount>>(json) ?? new List<GameAccount>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"加载游戏账号失败: {ex.Message}");
                return new List<GameAccount>();
            }
        }

        /// <summary>
        /// 保存游戏账号信息到 JSON 文件
        /// </summary>
        private void SaveGameAccounts()
        {
            try
            {
                string json = JsonSerializer.Serialize(_gameAccounts, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_dataFilePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"保存游戏账号失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 计算字符串的 SHA256 哈希
        /// </summary>
        /// <param name="rawData">原始字符串</param>
        /// <returns>SHA256 哈希字符串</returns>
        private string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // 计算哈希
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // 转换为字符串
                StringBuilder builder = new StringBuilder();
                foreach (var b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
