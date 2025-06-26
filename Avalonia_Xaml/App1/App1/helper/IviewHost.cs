using System.Threading.Tasks;
using App1.ViewModels;

public interface IViewHost
{
    ViewModelBase LeftView { get; set; }
    ViewModelBase RightView { get; set; }

    Task NavigateLeft(ViewModelBase viewModel);
    Task NavigateRight(ViewModelBase viewModel);
}