/*
 * Copyright (c) 2011 Original Authors
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
package com.sharneng.webservlet;

import org.springframework.web.context.WebApplicationContext;
import org.springframework.web.context.support.WebApplicationContextUtils;

import javax.servlet.ServletConfig;
import javax.servlet.ServletException;

/**
 * This is a binding Servlet that delegates its work to a {@link WebServlet} declared in Spring framework's
 * {@link WebApplicationContext} . Servlet init parameter "WebServletName" (the value of
 * {@link #WEB_SERVLET_NAME_PARAMETER}) should be used to specify which bean in the spring context to delegate the work.
 * <p>
 * Below is an example using {@code SpringBinder} to declare a {@code WebServlet} bean, named "helloWorldServlet" in the
 * Spring's context, to be a servlet.
 * 
 * <pre>
 * {@code
 * <servlet>
 *     <servlet-name>Hello World Servlet</servlet-name>
 *     <servlet-class>com.sharneng.webservlet.SpringBinder</servlet-class>
 *     <init-param>
 *         <param-name>WebServletName</param-name>
 *         <param-value>helloWorldServlet</param-value>
 *     </init-param>
 * </servlet>
 * }
 * </pre>
 * 
 * @author Kenneth Xu
 * 
 */
public class SpringBinder extends AbstractBinder {
    /** Name of the servlet init parameter that to specify a WebServlet bean in Spring context. */
    public static final String WEB_SERVLET_NAME_PARAMETER = "WebServletName";
    private static final long serialVersionUID = 1L;

    @Override
    protected WebServlet getWebServlet(ServletConfig config) throws ServletException {
        String webServletName = config.getInitParameter(WEB_SERVLET_NAME_PARAMETER);
        final WebApplicationContext context = WebApplicationContextUtils.getWebApplicationContext(config
                .getServletContext());
        return (WebServlet) context.getBean(webServletName);
    }
}
