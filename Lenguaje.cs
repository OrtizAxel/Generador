//Axel Ortiz Ricalde
using System;
using System.Collections.Generic;

//Requerimiento 1: Construir un metodo para escribir en el archivo lenguaje.cs identando el codigo
//                 { incrementa un tabulador, } decrementa un tabulador (hecho)
//Requerimiento 2: Declarar un atributo primeraProduccion de tipo string y actualizarlo con la primera
//                 produccion de la gramatica (hecho)
//Requerimiento 3: La primera produccion es publica y el resto privadas (hecho)
//Requerimiento 4: El constructor Lexico() parametrizado debe validar que la extension del archivo a complilar
//                 sea .gen y si no levanta una excepcion
//Requerimiento 5: Resolver la ambiguedad de ST y SNT
namespace Generador
{
    public class Lenguaje : Sintaxis, IDisposable
    {
        int contTab;
        string primeraProduccion;
        bool produccionPublica;


        public Lenguaje(string nombre) : base(nombre)
        {
            contTab = 0;
            primeraProduccion = "";
            produccionPublica = true;
        }
        public Lenguaje()
        {
            contTab = 0;
            primeraProduccion = "";
            produccionPublica = true;
        }
        public void Dispose()
        {
            cerrar();
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
        public void gramatica()
        {
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
            match(Tipos.SNT);
            match(Tipos.FinProduccion);
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
            match(Tipos.SNT);
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
            if(esTipo(getContenido()))
            {
                lenguaje.WriteLine(tabula() + "match(Tipos." + getContenido() + ");");
                match(Tipos.SNT);
            }
            else if(getClasificacion() == Tipos.ST)
            {
                lenguaje.WriteLine(tabula() + "match(\"" + getContenido() + "\");");
                match(Tipos.ST);
            }
            else if(getClasificacion() == Tipos.SNT)
            {
                lenguaje.WriteLine(tabula() + getContenido() + "();");
                match(Tipos.SNT);
            }
            else
            {
                throw new Exception("Error de sintaxis");
            }
            if(getClasificacion() != Tipos.FinProduccion)
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
                case "caseZona":
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
                tab += "\t";
            }
            return tab;
        }
    }
}