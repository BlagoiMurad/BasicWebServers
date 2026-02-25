using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicWebServer.Server.Views
{
    public class formView
    {
        public const string HTML = @"<!doctype html>
<html lang=""bg"">
<head>
  <meta charset=""utf-8"" />
  <meta name=""viewport"" content=""width=device-width, initial-scale=1"" />
  <title>Форма: Име, Възраст, Къде живее</title>
  <style>
    body { font-family: Arial, sans-serif; padding: 24px; max-width: 720px; margin: 0 auto; }
    h1 { margin: 0 0 16px; }
    form { display: grid; gap: 12px; padding: 16px; border: 1px solid #ddd; border-radius: 10px; }
    label { font-weight: 600; }
    input { padding: 10px; border: 1px solid #ccc; border-radius: 8px; font-size: 16px; }
    button { padding: 10px 14px; border: 0; border-radius: 8px; font-size: 16px; cursor: pointer; }
    .row { display: grid; gap: 6px; }
    .result { margin-top: 16px; padding: 14px; border: 1px dashed #aaa; border-radius: 10px; }
    .error { color: #b00020; margin: 6px 0 0; font-size: 14px; }
  </style>
</head>
<body>
  <h1>Въведи данни</h1>

  <form id=""personForm"" action='/HTML' method='Post'>
    <div class=""row"">
      <label for=""name"">Name</label>
      <input id=""name"" name=""name"" type=""text"" placeholder=""Пример: Martin"" required minlength=""2"" />
      <div class=""error"" id=""nameErr"" aria-live=""polite""></div>
    </div>

    <div class=""row"">
      <label for=""age"">Age</label>
      <input id=""age"" name=""age"" type=""number"" placeholder=""Пример: 16"" required min=""0"" max=""120"" />
      <div class=""error"" id=""ageErr"" aria-live=""polite""></div>
    </div>

    <div class=""row"">
      <label for=""city"">Къде живее</label>
      <input id=""city"" name=""city"" type=""text"" placeholder=""Пример: София"" required minlength=""2"" />
      <div class=""error"" id=""cityErr"" aria-live=""polite""></div>
    </div>

    <button type=""submit"">Запази</button>
  </form>

  <div class=""result"" id=""result"" style=""display:none;""></div>

  
</body>
</html>
";
    }

}
