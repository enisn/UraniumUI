using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UraniumUI.WebComponents.Controls;
public interface ICodeView
{
    string SourceCode { get; set; }

    string Language { get; set; }

    string Theme { get; set; }
}
