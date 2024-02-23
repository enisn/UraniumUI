using InputKit.Shared.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace UraniumUI.Options;
public class AutoFormViewOptions
{
    public Dictionary<Type, EditorForType> EditorMapping { get; } = new();

    public delegate View EditorForType(PropertyInfo property, object source);
}
