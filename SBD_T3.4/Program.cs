using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace SBD_T3._4
{
    class Program
    {
        static void Main(string[] args)
        {
            var undoList = new List<string>();
            var redoList = new List<string>();
            var lstLogArchive = new List<string>();
            var lList = new List<string>();
            string arquivo = @"c:\users\gilse\documents\visual studio 2013\Projects\SBD_T3.4\SBD_T3.4\Arquivo.log";
            if (File.Exists(arquivo))
            {
                try
                {
                    using (StreamReader sr = new StreamReader(arquivo))
                    {
                        string linha;
                        while ((linha = sr.ReadLine()) != null)
                        {
                            lstLogArchive.Add(linha);
                        }

                        for (int i = lstLogArchive.Count-1; i>= 0; i--)
                        {
                            //Examina-se o log de trás para frente - Até encontrar o primeiro checkpoint [stack]
                            if (lstLogArchive[i].Contains("checkpoint"))
                            {
                                //Qdo todos os registros de log apropriados tiverem sido examinados
                                //Checar a lista L no registro de checkpoint em questão
                                string transactions = ReplaceString.Remove(lstLogArchive[i], "<", "checkpoint", " ", ">");
                                lList.AddRange(transactions.Split(',').ToList());

                                //Para cada Ti em L
                                //Se Ti não estiver na lista refazer - Adicionar Ti à lista inutilizar
                                foreach (var transaction in lList)
                                {
                                    if (!redoList.Contains(transaction))
                                        undoList.Add(transaction);
                                }

                                break;
                            }

                            //Para cada registro <Ti commit> - Adicionar Ti à lista refazer
                            else if (lstLogArchive[i].Contains("commit"))
                            {
                                string transaction = ReplaceString.Remove(lstLogArchive[i], "<", "commit", " ", ">");
                                redoList.Add(transaction);
                            }

                            //Para cada registro <Ti start> - Se Ti não estiver na lista refazer - Adicionar à lista inutilizar
                            else if (lstLogArchive[i].Contains("start"))
                            {
                                string transaction = ReplaceString.Remove(lstLogArchive[i], "<", "start", " ", ">");
                                if(!redoList.Contains(transaction))
                                    undoList.Add(transaction);
                            }

                        }

                        //Uma vez construídas as listas:
                        //Reexaminar o log a partir do registro mais recente
                        //processar undo para cada registro de log pertencente à transação
                        //Ti na lista inutilizar (registros de log de transações na lista refazer são
                        //ignorados nesta fase)
                        //O exame pára qdo os registros <Ti start> são encontrados para
                        //cada transação Ti na lista inutilizar

                        if (undoList.Count > 0)
                            Console.WriteLine("Undo: {0}", String.Join(", ", undoList.ToArray()));
                        if (undoList.Count > 0)
                            Console.WriteLine("Redo: {0}", String.Join(", ", redoList.ToArray()));

                        if (undoList.Count > 0)
                        {
                            Console.WriteLine("\tPrimeiramente: Desfazer :");
                            for (int i = lstLogArchive.Count - 1; i >= 0; i--)
                            {
                                foreach (var undo in undoList)
                                {
                                    if (lstLogArchive[i].Contains("start") && lstLogArchive[i].Contains(undo))
                                        break;

                                    if (lstLogArchive[i].Contains("write") && lstLogArchive[i].Contains(undo))
                                        Console.WriteLine("\t\t{0}", lstLogArchive[i]);
                                }
                            }
                        }

                        //Localizar o registro <checkpoint L> mais recente no log
                        //Examinar o log a partir do registro <checkpoint L> mais recente até o final
                        //processar redo para cada registro de log pertencente a uma
                        //    transação Ti que está na lista refazer (ignorar os registros de log
                        //    de transações na lista inutilizar nesta fase)

                        if (redoList.Count > 0)
                        {
                            int posCheckPoint;
                            for (posCheckPoint = lstLogArchive.Count - 1; posCheckPoint >= 0; posCheckPoint--)
                            {
                                if (lstLogArchive[posCheckPoint].Contains("checkpoint"))
                                    break;
                            }

                            //Para pegar a posição seguinte do checkpoint
                            posCheckPoint++;

                            Console.WriteLine("\tApós: Refazer:");
                            for (int i = posCheckPoint; i < lstLogArchive.Count; i++)
                            {
                                foreach (var redo in redoList)
                                {
                                    if ((lstLogArchive[i].Contains("write") || lstLogArchive[i].Contains("commit")) && lstLogArchive[i].Contains(redo))
                                        Console.WriteLine("\t\t{0}", lstLogArchive[i]);
                                }
                            }
                        }

                        Console.WriteLine("\n\nSistema recuperado da falha...\n\n");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            else
            {
                Console.WriteLine(" O arquivo " + arquivo + " não foi localizado !");
            }
            Console.ReadKey();
        }
    }
}