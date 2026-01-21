using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using UraniumUI;

namespace UraniumApp.ViewModels
{
    public class TreeViewPageViewModel : UraniumBindableObject
    {
        private MyItem selectedNode;
        private string selectedNodeName = "None";

        public ObservableCollection<MyItem> Nodes { get; set; } = new();

        public ICommand ExpandCommand { get; set; }
        public ICommand NodeSelectedCommand { get; set; }

        public MyItem SelectedNode
        {
            get => selectedNode;
            set => SetProperty(ref selectedNode, value);
        }

        public string SelectedNodeName
        {
            get => selectedNodeName;
            set => SetProperty(ref selectedNodeName, value);
        }

        public TreeViewPageViewModel()
        {
            Nodes.Add(new MyItem("A")
            {
                Children =
                {
                    new MyItem("A.1"),
                    new MyItem("A.2"),
                }
            });
            Nodes.Add(new MyItem("B")
            {
                IsExtended = true,
                Children =
                {
                    new MyItem("B.1")
                    {
                        IsExtended = true,
                        Children =
                        {
                            new MyItem("B.1.a"),
                            new MyItem("B.1.b"),
                            new MyItem("B.1.c"),
                            new MyItem("B.1.d"),

                        }
                    },
                    new MyItem("B.2"),
                }
            });
            Nodes.Add(new MyItem("C"));
            Nodes.Add(new MyItem("D"));

            ExpandCommand = new Command(() =>
            {
                App.Current.MainPage.DisplayAlert("S", Nodes.ToString(), "ok");
            });

            NodeSelectedCommand = new Command<MyItem>((node) =>
            {
                if (node != null)
                {
                    SelectedNodeName = node.Name;
                }
                else
                {
                    SelectedNodeName = "None";
                }
            });
        }
    }

    public class MyItem : UraniumBindableObject
    {
        private string name;
        private bool isExtended;
        private object value;
        public MyItem()
        {

        }

        public MyItem(string name)
        {
            Name = name;
        }

        public virtual string Name { get => name; set => SetProperty(ref name, value); }
        public virtual bool IsExtended { get => isExtended; set => SetProperty(ref isExtended, value); }
        public virtual object Value { get => value; set => SetProperty(ref this.value, value); }
        public virtual IList<MyItem> Children { get; set; } = new ObservableCollection<MyItem>();
        public bool IsLeaf => Children.Count == 0;
    }
}
