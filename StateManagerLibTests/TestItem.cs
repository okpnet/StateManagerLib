using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StateManagerLibTests
{
    public class TestItem:INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        TestItem? parent;
        public TestItem? Parent
        {
            get => parent;
            set
            {
                parent = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TestItem.Parent)));
            }
        }


        string? name;
        public string? Name 
        {
            get => name;
            set
            {
                name = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TestItem.Name)));
            }
        }
        int id;
        public int Id 
        {
            get => id;
            set
            {
                id = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TestItem.Id)));
            }
        }
        ObservableCollection<TestItem>? children;
        public ObservableCollection<TestItem>? Children 
        {
            get => children;
            set
            {
                children = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TestItem.Children)));
            }
        }
    }
}
