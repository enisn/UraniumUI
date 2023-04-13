using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UraniumUI.StyleBuilder.ViewModels;
public interface ISavable
{
    Task SaveAsync();

    Task SaveAsAsync();

    string Title { get; }
}
