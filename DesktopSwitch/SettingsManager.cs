using System;
using System.IO;
using Newtonsoft.Json.Linq;

namespace DesktopSwitch
{
  public class SettingsManager
  {
    private string _fileName;
    private JObject _settings;

    public void Init(string fileName)
    {
      _fileName = fileName;

      if (!File.Exists(fileName))
      {
        _settings = new JObject();
        return;
      }
      var rawSettings = File.ReadAllText(fileName);
      _settings = JObject.Parse(rawSettings);
    }

    public Lazy<T> GetOrNew<T>() where T : class, new()
    {
      return new Lazy<T>(() =>
      {
        var jToken = _settings[typeof (T).Name];
        return jToken != null ? jToken.ToObject<T>() : new T();
      });
    }

    public EditContext<T> GetEditContext<T>() where T : class, new()
    {
      return new EditContext<T>(this, GetOrNew<T>().Value);
    }

    private void SaveValue<T>(T value)
    {
      var name = typeof (T).Name;
//      _settings.Add(new JProperty(name, JObject.FromObject(value)));
//      _settings[name] = new JValue(value);
      _settings[name] = JObject.FromObject(value);

      File.WriteAllText(_fileName, _settings.ToString());
    }

    public class EditContext<T> : IDisposable
    {
      private SettingsManager _owner;

      public EditContext(SettingsManager owner, T value)
      {
        _owner = owner;
        Value = value;
      }

      public T Value;

      public void Dispose()
      {
        _owner.SaveValue<T>(Value);
      }
    }
  }

  
}
