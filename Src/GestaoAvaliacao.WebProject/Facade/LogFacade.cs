using GestaoAvaliacao.Util;
using GestaoAvaliacao.WebProject.Entities;
using MSTech.CoreSSO.BLL;
using MSTech.CoreSSO.Entities;
using MSTech.Log;
using System;
using System.Web;

namespace GestaoAvaliacao.WebProject.Facade
{
    public static class LogFacade
    {
        private static string path = String.Concat(System.Web.HttpContext.Current.Request.PhysicalApplicationPath, "Log");

        #region Public methods

        /// <summary>
        /// Logs personalizable errors that are thrown on routines in this web site.
        /// In case of error, tries to log the exception on error file.
        /// </summary>
        public static void SaveBasicError(string errorMessage, string errorType = null)
        {
            LOG_Erros entity = new LOG_Erros();
            try
            {
                entity.err_descricao = errorMessage;
                entity.err_tipoErro = errorType;
                entity.err_dataHora = DateTime.Now;
                if (HttpContext.Current != null && HttpContext.Current.Request != null)
                {
                    entity.err_ip = HttpContext.Current.Request.UserHostAddress;
                    entity.err_machineName = HttpContext.Current.Server.MachineName;
                    entity.err_caminhoArq = HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath;
                    try
                    {
                        entity.err_browser = String.Concat(new[] { HttpContext.Current.Request.Browser.Browser, HttpContext.Current.Request.Browser.MajorVersion.ToString(), HttpContext.Current.Request.Browser.MinorVersionString });
                    }
                    catch
                    {
                        entity.err_browser = string.Empty;
                    }

                    SYS_Sistema sistema = SYS_SistemaBO.GetEntity(new SYS_Sistema() { sis_id = GestaoAvaliacao.Util.Constants.IdSistema });
                    entity.sis_id = sistema.sis_id;
                    entity.sis_decricao = sistema.sis_nome;

                    if (HttpContext.Current.Session != null)
                    {
                        UsuarioLogado session = SessionFacade.UsuarioLogado;
                        if (session != null && session.Usuario != null)
                        {
                            entity.usu_id = session.Usuario.usu_id;
                            entity.usu_login = session.Usuario.usu_login;
                        }
                    }
                }

                LOG_ErrosBO.Save(entity);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static void SaveError(Exception ex, string physicalDirectory, string error = null)
        {
            try
            {
                var logError = new LogError(string.Concat(physicalDirectory, "Log"))
                {
                    SaveLogBD = delegate (string message)
                    {
                        var entity = new LOG_Erros
                        {
                            err_descricao = message
                        };

                        if (ex != null)
                        {
                            entity.err_erroBase = ex.GetBaseException().Message;
                            entity.err_tipoErro = ex.GetBaseException().GetType().FullName;
                        }
                        else
                        {
                            entity.err_erroBase = error;
                        }

                        entity.err_dataHora = DateTime.Now;

                        if (HttpContext.Current != null && HttpContext.Current.Request != null)
                        {
                            entity.err_ip = HttpContext.Current.Request.UserHostAddress;
                            entity.err_machineName = HttpContext.Current.Server.MachineName;
                            entity.err_caminhoArq = HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath;

                            try
                            {
                                entity.err_browser = string.Concat(new[]
                                {
                                    HttpContext.Current.Request.Browser.Browser,
                                    HttpContext.Current.Request.Browser.MajorVersion.ToString(),
                                    HttpContext.Current.Request.Browser.MinorVersionString
                                });
                            }
                            catch
                            {
                                entity.err_browser = string.Empty;
                            }

                            var sistema = SYS_SistemaBO.GetEntity(new SYS_Sistema { sis_id = Constants.IdSistema });

                            entity.sis_id = sistema.sis_id;
                            entity.sis_decricao = sistema.sis_nome;

                            if (HttpContext.Current.Session != null)
                            {
                                var session = SessionFacade.UsuarioLogado;

                                if (session?.Usuario != null)
                                {
                                    entity.usu_id = session.Usuario.usu_id;
                                    entity.usu_login = session.Usuario.usu_login;
                                }
                            }
                        }

                        LOG_ErrosBO.Save(entity);
                    }
                };

                if (ex != null)
                    logError.Log(ex, true);
                else
                    logError.Log(error, true);
            }
            catch
            {
                // Sem tratamento
            }
        }

        /// <summary>
        /// Logs exceptions that are thrown on routines in this web site.
        /// In case of error, tries to log the exception on error file.
        /// </summary>
        /// <param name="ex">Exception to be saved on database</param>
        /// <param name="error"></param>
        public static void SaveError(Exception ex, string error = null)
        {
            SaveError(ex, ApplicationFacade.PhysicalDirectory, error);
        }

        public static void SaveErrorSme(Exception ex, string error = null)
        {
            SaveError(ex, ApplicationFacade.PhysicalDirectorySme, error);
        }

        /// <summary>
        /// Save the system log, means saving an action that the user did 
        /// in the web site.
        /// </summary>
        /// <param name="action">Action executed by user</param>
        /// <param name="description">Log description</param>
        /// <returns>Reports whether the log was saved successfully</returns>
        public static Guid SaveSystemLog(LOG_SistemaTipo action, string description)
        {
            try
            {
                LOG_Sistema entity = new LOG_Sistema();
                entity.log_acao = Enum.GetName(typeof(LOG_SistemaTipo), action);
                entity.log_dataHora = DateTime.Now;
                entity.log_descricao = description;

                //Preenche os dados do sistema
                SYS_Sistema sistema = SYS_SistemaBO.GetEntity(new SYS_Sistema() { sis_id = Constants.IdSistema });
                entity.sis_id = sistema.sis_id;
                entity.sis_nome = sistema.sis_nome;

                if (HttpContext.Current != null)
                {
                    //Preenche dados do host do site                    
                    LOG_SistemaBO.GenerateLogID();
                    entity.log_id = new Guid(HttpContext.Current.Session[LOG_Sistema.SessionName].ToString());
                    entity.log_ip = HttpContext.Current.Request.UserHostAddress;
                    entity.log_machineName = HttpContext.Current.Server.MachineName;

                    if (HttpContext.Current.Session != null)
                    {
                        UsuarioLogado session = SessionFacade.UsuarioLogado;
                        if (session != null)
                        {
                            //Preenche dados referente ao usuário
                            if (session.Usuario != null)
                            {
                                entity.usu_id = session.Usuario.usu_id;
                                entity.usu_login = session.Usuario.usu_login;
                            }
                            //Preenche dados referente ao grupo do usuário
                            if (session.Grupo != null)
                            {
                                //Preenche os dados do grupo
                                entity.gru_id = session.Grupo.gru_id;
                                entity.gru_nome = session.Grupo.gru_nome;

                                //Preenche os dados do módulo
                                SYS_Modulo modulo = (SYS_Modulo)HttpContext.Current.Session[SYS_Modulo.SessionName];
                                if (modulo != null)
                                {
                                    entity.mod_id = modulo.mod_id;
                                    entity.mod_nome = modulo.mod_nome;
                                }
                            }
                        }
                    }
                }

                if (!LOG_SistemaBO.Save(entity))
                {
                    throw new Exception("Unable to save the system log.");
                }
                return entity.log_id;
            }
            finally
            {
                if (HttpContext.Current != null)
                {
                    HttpContext.Current.Session[LOG_Sistema.SessionName] = null;
                }
            }
        }

        #endregion
    }
}
