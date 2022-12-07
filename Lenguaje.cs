//Axel Ortiz Ricalde
using System;
using System.Collections.Generic;

//Requerimiento 1: Construir un metodo para escribir en el archivo lenguaje.cs identando el codigo
//                 { incrementa un tabulador, } decrementa un tabulador (hecho)
//Requerimiento 2: Declarar un atributo primeraProduccion de tipo string y actualizarlo con la primera
//                 produccion de la gramatica (hecho)
//Requerimiento 3: La primera produccion es publica y el resto privadas (hecho)
//Requerimiento 4: El constructor Lexico() parametrizado debe validar que la extension del archivo a complilar
//                 sea .gen y si no levanta una excepcion (hecho)
//Requerimiento 5: Resolver la ambiguedad de ST y SNT
//                 Recorrer linea por linea el archivo gram para extraer el nombre de cada produccion (hecho)
//Requerimiento 6: Agregar el parentesis izquierdo y derecho escapados en la matriz de transiciones
//Requerimiento 7: Implementar el or y la cerradura epsilon 
//
namespace Generador
{
    public class Lenguaje : Sintaxis, IDisposable
    {
        int contTab;
        string primeraProduccion;
        bool produccionPublica;
        List<string> listaSNT;

        public Lenguaje(string nombre) : base(nombre)
        {
            contTab = 0;
            primeraProduccion = "";
            produccionPublica = true;
            listaSNT = new List<string>();
        }
        public Lenguaje()
        {
            contTab = 0;
            primeraProduccion = "";
            produccionPublica = true;
            listaSNT = new List<string>();
        }
        public void Dispose()
        {
            cerrar();
        }
        
        private bool esSNT(string contenido)
        {
            return listaSNT.Contains(contenido);
        }
        private void agregarSNT()
        {
            while(!FinArchivo())
            {
                listaSNT.Add(getContenido());
                archivo.ReadLine();
                NextToken();
            }
            archivo.DiscardBufferedData();
            archivo.BaseStream.Seek(0, SeekOrigin.Begin);
            NextToken();
        }
        
        public void gramatica()
        { 
            archivo.ReadLine();
            NextToken();
            agregarSNT();
            contTab = 0;
            cabecera();
            primeraProduccion = getContenido();
            Programa(primeraProduccion);
            cabeceraLenguaje();
            listaProducciones();
            contTab = 1;
            lenguaje.WriteLine(tabula() + "}");
            lenguaje.WriteLine("}");  
        }
        private void cabecera()
        {
            match("Gramatica");
            match(":");
            match(Tipos.ST);
            match(Tipos.FinProduccion);
        }
        private void Programa(string produccionPrincipal)
        {
            contTab = 0;
            programa.WriteLine("using System;");
            programa.WriteLine("using System.IO;");
            programa.WriteLine("using System.Collections.Generic;");
            programa.WriteLine();
            programa.WriteLine("namespace Generico");
            programa.WriteLine("{");
            contTab++;
            programa.WriteLine(tabula() + "public class Program");
            programa.WriteLine(tabula() + "{");
            contTab++;
            programa.WriteLine(tabula() + "static void Main(string[] args)");
            programa.WriteLine(tabula() + "{");
            contTab++;
            programa.WriteLine(tabula() + "try");
            programa.WriteLine(tabula() + "{");
            contTab++;
            programa.WriteLine(tabula() + "using (Lenguaje a = new Lenguaje())");
            programa.WriteLine(tabula() + "{");
            contTab++;
            programa.WriteLine(tabula() + "a." + produccionPrincipal + "();");
            contTab--;
            programa.WriteLine(tabula() + "}");
            contTab--;
            programa.WriteLine(tabula() + "}");
            programa.WriteLine(tabula() + "catch (Exception e)");
            programa.WriteLine(tabula() + "{");
            contTab++;
            programa.WriteLine(tabula() + "Console.WriteLine(e.Message);");
            contTab--;
            programa.WriteLine(tabula() + "}");
            contTab--;
            programa.WriteLine(tabula() + "}");
            contTab--;
            programa.WriteLine(tabula() + "}");
            contTab--;
            programa.WriteLine(tabula() + "}");
        }
        private void cabeceraLenguaje()
        {
            contTab = 0;
            lenguaje.WriteLine("using System;");
            lenguaje.WriteLine("using System.Collections.Generic;");
            lenguaje.WriteLine("namespace Generico");
            lenguaje.WriteLine("{");
            contTab++;
            lenguaje.WriteLine(tabula() + "public class Lenguaje : Sintaxis, IDisposable");
            lenguaje.WriteLine(tabula() + "{");
            contTab++;
            lenguaje.WriteLine(tabula() + "public Lenguaje(string nombre) : base(nombre)");
            lenguaje.WriteLine(tabula() + "{");
            contTab++;
            contTab--;
            lenguaje.WriteLine(tabula() + "}");
            lenguaje.WriteLine(tabula() + "public Lenguaje()");
            lenguaje.WriteLine(tabula() + "{");
            contTab++;
            contTab--;
            lenguaje.WriteLine(tabula() + "}");
            lenguaje.WriteLine(tabula() + "public void Dispose()");
            lenguaje.WriteLine(tabula() + "{");
            contTab++;
            lenguaje.WriteLine(tabula() + "cerrar();");
            contTab--;
            lenguaje.WriteLine(tabula() + "}");
        }
        private void listaProducciones()
        {
            contTab = 2;
            if(produccionPublica)
            {
                lenguaje.WriteLine(tabula() + "public void " + getContenido() + "()");
                produccionPublica = false;
            }
            else
            {
                lenguaje.WriteLine(tabula() + "private void " + getContenido() + "()");
            }
            lenguaje.WriteLine(tabula() + "{");
            contTab++;
            match(Tipos.ST);
            match(Tipos.Produce);
            simbolos();
            match(Tipos.FinProduccion);
            contTab--;
            lenguaje.WriteLine(tabula() + "}");
            if (!FinArchivo())
            {
                listaProducciones();
            }
        }

        private void simbolos()
        {
            if(getContenido() == "\\(")
            {
                match("\\(");
                if(esSNT(getContenido()))
                {
                    throw new Exception("Error: Se espera un simbolo terminal");
                }
                if(esTipo(getContenido()))
                {
                    lenguaje.WriteLine(tabula() + "if(getClasificacion() == Tipos." + getContenido() + ")");
                }
                else
                {
                    lenguaje.WriteLine(tabula() + "if(getContenido() == \"" + getContenido() + "\")");
                }
                lenguaje.WriteLine(tabula() + "{");
                contTab++;
                simbolos();
                match("\\)");
                contTab--;
                lenguaje.WriteLine(tabula() + "}");
            }
            else if(esTipo(getContenido()))
            {
                lenguaje.WriteLine(tabula() + "match(Tipos." + getContenido() + ");");
                match(Tipos.ST);
            }
            else if(esSNT(getContenido()))
            {
                lenguaje.WriteLine(tabula() + getContenido() + "();");
                match(Tipos.ST);
            }
            else if(getClasificacion() == Tipos.ST)
            {
                lenguaje.WriteLine(tabula() + "match(\"" + getContenido() + "\");"); 
                match(Tipos.ST);
            }        
            if(getClasificacion() != Tipos.FinProduccion && getContenido() != "\\)")
            {
                simbolos();
            }
        }

        private bool esTipo(string clasificacion)
        {
            switch(clasificacion)
            {
                case "Identificador":
                case "Numero":
                case "Caracter":
                case "Asignacion":
                case "Inicializacion":
                case "OperadorLogico":
                case "OperadorRelacional":
                case "OperadorTernario":
                case "OperadorTermino":
                case "OperadorFactor":
                case "IncrementoTermino":
                case "IncrementoFactor":
                case "FinSentencia":
                case "Cadena":
                case "TipoDato":
                case "Zona":
                case "Condicion":
                case "Ciclo":
                    return true;
            }
            return false;
        }

        private string tabula()
        {
            string tab = "";
            for(int i = 0; i < contTab; i++)
            {
                tab += "    ";
            }
            return tab;
        }
    }
}