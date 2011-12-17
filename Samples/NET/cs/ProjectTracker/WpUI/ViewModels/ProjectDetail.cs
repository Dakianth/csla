﻿using System.Collections.ObjectModel;

namespace WpUI.ViewModels
{
  public class ProjectDetail : ViewModel<ProjectTracker.Library.ProjectGetter>
  {
    public ProjectDetail(string queryString)
    {
      var p = queryString.Split('=');
      var projectId = int.Parse(p[1]);
      ManageObjectLifetime = false;
      BeginRefresh(callback => ProjectTracker.Library.ProjectGetter.GetExistingProject(projectId, callback));
    }

    protected override void OnModelChanged(ProjectTracker.Library.ProjectGetter oldValue, ProjectTracker.Library.ProjectGetter newValue)
    {
      base.OnModelChanged(oldValue, newValue);
      OnPropertyChanged("Resources");
    }

    public ObservableCollection<ResourceInfo> Resources
    {
      get 
      {
        var result = new ObservableCollection<ResourceInfo>();
        if (Model != null)
          foreach (var item in Model.Project.Resources)
            result.Add(new ResourceInfo(item));
        return result;
      }
    }

    public void Delete()
    {
      Model.Project.Delete();
      App.ViewModel.ShowStatus(new Bxf.Status { IsBusy = true, Text = "Deleting project" });
      Model.Project.BeginSave((o, e) =>
        {
          App.ViewModel.ShowStatus(new Bxf.Status());
          if (e.Error != null)
            App.ViewModel.ShowError(e.Error.Message, "Project delete");
          else
            App.ViewModel.ShowView(null);
        });
    }

    public void Edit()
    {
      App.ViewModel.ShowView("/ProjectEdit.xaml?id=" + Model.Project.Id);
    }

    public void Close()
    {
      App.ViewModel.ShowView(null);
    }

    public class ResourceInfo : ViewModelLocal<ProjectTracker.Library.ProjectResourceEdit>
    {
      public ResourceInfo(ProjectTracker.Library.ProjectResourceEdit model)
      {
        ManageObjectLifetime = false;
        Model = model;
      }

      public void ShowResource()
      {
        App.ViewModel.ShowView("/ResourceDetails.xaml?id=" + Model.ResourceId);
      }
    }
  }
}