using LLM.Models;
using System;
using System.Collections.Generic;
namespace LLM.Utilities;

public struct DouBao
{
    public static string Lite_4K = "ep-20241228185650-fgkk4";
    public static string Lite_32K = "ep-20241228233018-lrk8p";
    public static string Lite_128K = "ep-20241228185618-qzwsh";
    public static string Pro_4K = "ep-20241228185547-7xmhf";
    public static string Pro_32K = "ep-20241228185502-dlp5n";
    public static string Pro_128K = "ep-20241228185524-w2gfk";
    public static string Pro_256K = "ep-20241228185207-trz99";
    public static string deepseek_r1_671b = "deepseek-r1-250120";
}

public class MultiAPIPriceCalculator
{

    private const decimal USD_TO_RMB_RATE = 7.0m;

    private static readonly Dictionary<LLMType, Dictionary<string, (decimal inputPricePerKToken, decimal outputPricePerKToken)>> _modelPrices = new()
    {
        {
            LLMType.ChatGpt, new Dictionary<string, (decimal, decimal)>
            {
                { "gpt-4o-mini", (0.000150m, 0.000600m) },
                { "gpt-4o", (0.00250m,0.01000m) },
                { "o1-mini", (0.0030m,0.0120m) },
                { "o1", (0.0150m, 0.0600m) },
            }
        },
        {
            LLMType.豆包, new Dictionary<string, (decimal, decimal)>
            {
                { DouBao.Lite_4K, (0.0003m, 0.0006m) },//lite-4k
                { DouBao.Lite_32K, (0.0003m, 0.0006m) },//lite-4k
                { DouBao.Lite_128K, (0.0008m, 0.0010m) },//lite-128k
                { DouBao.Pro_4K, (0.0008m, 0.0020m) },//pro-4k
                { DouBao.Pro_32K, (0.0008m, 0.0020m) },//pro-32k
                { DouBao.Pro_128K, (0.0050m, 0.0090m) },//pro-128k
                { DouBao.Pro_256K, (0.0050m, 0.0090m) },//pro-256k
                { "deepseek-r1-250120", (0.0050m, 0.0090m) },//pro-256k
            }
        },
        {
            LLMType.通义千问, new Dictionary<string, (decimal, decimal)>
            {
                { "qwen-max", (0.02m, 0.06m) },
                { "qwen-plus", (0.0008m, 0.002m) },
                { "qwen-turbo", (0.0003m, 0.0006m) },
                { "qwen-long", (0.0005m, 0.002m) },
            }
        },
        {
            LLMType.Ollama, new Dictionary<string, (decimal, decimal)>
            {
                { "qwen2.5:7b", (0.000070m, 0.000300m) },
                { "deepseek-coder-v2:latest", (0.000070m, 0.000300m) },
                { "deepseek-r1:latest", (0.000070m, 0.000300m) },
            }
        }
    };

    public static decimal CalculateTotalCost(LLMType lLMType, string model, int promptTokens, int completionTokens)
    {
        if (!_modelPrices.ContainsKey(lLMType) || !_modelPrices[lLMType].ContainsKey(model))
        {
            //throw new ArgumentException("Invalid API category or model name");
            return 0;
        }

        // 获取模型的输入和输出价格
        var (inputPricePerKToken, outputPricePerKToken) = _modelPrices[lLMType][model];

        // 计算输入 Token 的费用
        decimal inputCost = promptTokens / 1000m * inputPricePerKToken;

        // 计算输出 Token 的费用
        decimal outputCost = completionTokens / 1000m * outputPricePerKToken;

        // 总费用是输入和输出费用之和
        decimal totalCost = inputCost + outputCost;


        if (lLMType == LLMType.ChatGpt)
        {
            totalCost *= USD_TO_RMB_RATE;
        }

        return totalCost;
    }

}
