using System;
using System.IO;
using System.Xml.Serialization;
namespace MuAtl.Service
{
  public interface IRepository
  {
    T Load<T>(string path);
    void Save<T>(string path, T data);
    event ProjectSavedEventHandler ProjectSaved;
    event ProjectLoadedEventHandler ProjectLoaded;
  }

  public delegate void ProjectSavedEventHandler();
  public delegate void ProjectLoadedEventHandler();

  public class Repository : IRepository
  {
    public Repository()
    {
    }

    public event ProjectSavedEventHandler ProjectSaved;
    public event ProjectLoadedEventHandler ProjectLoaded;

    public void Save<T>(string path, T data)
    {
      try
      {
        using (var file = new StreamWriter(path))
        {
          var serializer = new XmlSerializer(typeof(T));
          serializer.Serialize(file, data);
        }

        if(ProjectSaved != null)
        {
          ProjectSaved();
        }
      }
      catch (Exception ex)
      {
         throw ex;
      }
    }

    public T Load<T>(string path)
    {
      try
      {
        using (var file = new FileStream(path, FileMode.Open))
        {
          var serializer = new XmlSerializer(typeof(T));
          T project = (T)serializer.Deserialize(file);

          if (ProjectLoaded != null)
          {
            ProjectLoaded();
          }

          return project;
        }
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }
  }
}
