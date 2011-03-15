/*
 * Copyright (c) Contributors, http://opensimulator.org/
 * See CONTRIBUTORS.TXT for a full list of copyright holders.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *     * Redistributions of source code must retain the above copyright
 *       notice, this list of conditions and the following disclaimer.
 *     * Redistributions in binary form must reproduce the above copyright
 *       notice, this list of conditions and the following disclaimer in the
 *       documentation and/or other materials provided with the distribution.
 *     * Neither the name of the OpenSimulator Project nor the
 *       names of its contributors may be used to endorse or promote products
 *       derived from this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE DEVELOPERS ``AS IS'' AND ANY
 * EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL THE CONTRIBUTORS BE LIABLE FOR ANY
 * DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

using System;
using OpenMetaverse;
using OpenSim.Services.Interfaces;
using log4net;
using Nini.Config;
using System.Reflection;
using OpenSim.Data;
using OpenSim.Framework;
using OpenSim.Framework.Console;

namespace OpenSim.Services.AuthenticationService
{
    // Generic Authentication service used for identifying
    // and authenticating principals.
    // Principals may be clients acting on users' behalf,
    // or any other components that need 
    // verifiable identification.
    //
    public class WebkeyAuthenticationService :
            AuthenticationServiceBase, IAuthenticationService
    {
        private static readonly ILog m_log =
                LogManager.GetLogger(
                MethodBase.GetCurrentMethod().DeclaringType);

        public WebkeyAuthenticationService(IConfigSource config) :
            base(config)
        {
        }

        public string Authenticate(UUID principalID, string password, int lifetime)
        {
            m_log.InfoFormat("[Authenticate]: Trying a web key authenticate");
            if (new UUID(password) == UUID.Zero)
            {
                m_log.InfoFormat("[Authenticate]: NULL_KEY is not a valid web_login_key");
            }
            else
            {
                AuthenticationData data = m_Database.Get(principalID);
                if (data != null && data.Data != null)
                {
                    if (data.Data.ContainsKey("webLoginKey"))
                    {
                        m_log.InfoFormat("[Authenticate]: Trying a web key authentication");
						string key = data.Data["webLoginKey"].ToString();
						m_log.DebugFormat("[WEB LOGIN AUTH]: got {0} for key in db vs {1}", key, password);
						if (key == password)
						{
							data.Data["webLoginKey"] = UUID.Zero.ToString();
							m_Database.Store(data);
							return GetToken(principalID, lifetime);
						}
                    }else{
                        m_log.InfoFormat("[Authenticate]: no col webLoginKey in passwd.db");
                    }
                }
                m_log.DebugFormat("[AUTH SERVICE]: PrincipalID {0} or its data not found", principalID);
            }
            return String.Empty;
        }
    }
}
