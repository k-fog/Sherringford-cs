using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Sherringford.She.Ast
{
    static class ToDotGraph
    {
        public static string PlotDotGraph(this ASTree tree)
        {
            Type tt = tree.GetType();
            if (tt == typeof(ASTLeaf) || tt.IsSubclassOf(typeof(ASTLeaf))) return ((ASTLeaf)tree).PlotDotGraph();
            if (tt == typeof(ASTList) || tt.IsSubclassOf(typeof(ASTList))) return ((ASTList)tree).PlotDotGraph();
            return null;
        }

        public static string GetDotProperty(this ASTList tree)
        {
            return $"{tree.GetHashCode()} [label=\"{tree.GetType().Name}\", color=\"#92D7F4\", style=filled, shape=box]\n";
        }

        public static string GetDotProperty(this ASTLeaf leaf)
        {
            return $"{leaf.GetHashCode()} [label=\"{leaf.GetType().Name}:\n{Escape(leaf.Token.ToString())}\", color=\"#A8D281\", style=filled]\n";
        }

        public static string PlotDotGraph(this ASTList list)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var child in list)
            {
                sb.Append($"{list.GetHashCode()} -> {child.PlotDotGraph()}");
            }
            sb.Append(list.GetDotProperty());
            return sb.ToString();
        }

        public static string PlotDotGraph(this ASTLeaf leaf)
        {
            return $"{leaf.GetHashCode()}\n{leaf.GetDotProperty()}";
        }

        private static string Escape(string str)
        {
            return Regex.Replace(str, "\"", "\\\"");
        }
    }

    class ASTVisualizer
    {
        private StringBuilder top, body;

        public ASTVisualizer()
        {
            this.top = new StringBuilder();
            this.body = new StringBuilder();
        }

        public ASTVisualizer(ASTree ast)
        {
            this.body = new StringBuilder(ast.PlotDotGraph());
        }

        public void Push(ASTree ast)
        {
            top.Append(ast.GetHashCode());
            top.Append(" -> ");
            body.Append(ast.PlotDotGraph());
        }

        public void Visualize(string fileName)
        {
            top.Remove(top.Length - 3, 3);
            string dot = top.ToString();
            top.Replace(" ->", ";");
            string dot_p = "{rank=same;" + top.ToString() + "}\n";
            Visualize(dot + body + dot_p, fileName);
        }

        public static void Visualize(string source, string fileName)
        {
            //command : dot.exe .\graph.dot -Kdot -Tjpg -Nfontname="$font" -Efontname="$font" -Gfontname="$font" -T png -o sample.png
            //Ricty Diminished Discord
            using (var dotFile = new StreamWriter(fileName + ".dot", false))
            {
                dotFile.WriteLine("digraph d {");
                dotFile.WriteLine("graph[rankdir=\"LR\"];");
                dotFile.Write(source);
                dotFile.WriteLine("}");
            }
            string font = "Ricty Diminished Discord";
            ProcessStartInfo pInfo = new ProcessStartInfo();
            pInfo.FileName = @$"{SheInfo.ExePath}\Graphviz\bin\dot.exe";
            pInfo.Arguments = $"{fileName}.dot -Gdpi=300 -Kdot -Nfontname=\"{font}\" -Efontname=\"{font}\" -Gfontname=\"{font}\" -T png -o {fileName}.png";
            pInfo.UseShellExecute = true;

            Process.Start(pInfo);
        }
    }
}
