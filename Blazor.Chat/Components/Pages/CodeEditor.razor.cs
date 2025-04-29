using Blazor.Chat.Models;
using LLM.Services;

namespace Blazor.Chat.Components.Pages
{
    public partial class CodeEditor
    {
        //private async Task Ai()
        //{
        //    var person = await Fsql.Select<Person>().Where(p => p.ID == _person.ID).FirstAsync();

        //    if (person.Balance <= 0)
        //    {
        //        _infoMessage = "余额不足，无法进行AI操作！";
        //        _showInfoModal = true;
        //        return;
        //    }

        //    var content = await JS.InvokeAsync<string>("getEditorContent");

        //    var ret = await _lLMClient.SendMessageAsync(
        //        $"请根据以下代码和需求，修改代码。注意：每个方法上方都有 `FunctionDescription` 特性，格式为：第一个参数是函数名，第二个参数是功能描述；每个方法参数有 `ParameterDescription` 特性，描述参数用途，且所有参数和返回值都是 `string` 类型。请只提供修改后的代码，不需要额外说明。代码：{content}。需求：{_demand}",
        //        false
        //    );
        //    await Fsql.Update<Person>().Where(p => p.ID == _person.ID).Set(p => p.Balance - ret.Value.Item2).ExecuteAffrowsAsync();
        //    var res = ret.Value.Item1;
        //    if (res != null)
        //    {
        //        var msg = _lLMClient.ExtractMessages(res);
        //        var code = msg.Replace("```csharp", "").Replace("```", "");
        //        await JS.InvokeAsync<string>("updateEditorContent", code);

        //        while ()
        //        {

        //        }




        //        _infoMessage = "AI 编写完成！";
        //        _showInfoModal = true;
        //    }

        //    StateHasChanged();
        //}

    }
}
