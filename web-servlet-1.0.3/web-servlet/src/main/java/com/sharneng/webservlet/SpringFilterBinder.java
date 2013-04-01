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

import javax.servlet.Filter;
import javax.servlet.FilterConfig;
import javax.servlet.ServletException;

/**
 * This is a binding filter that delegates its work to a real filter declared in Spring framework's
 * {@link WebApplicationContext} . Servlet init parameter "WebFilterName" (the value of
 * {@link #WEB_FILTERT_NAME_PARAMETER}) should be used to specify which bean in the spring context to delegate the work.
 * <p>
 * Below is an example using {@code SpringFilterBinder} to declare a {@code WebServlet} bean, named "helloWorldServlet"
 * in the Spring's context, to be a filter.
 * 
 * <pre>
 * {@code
 * <filter>
 *     <filter-name>Logging Filter</filter-name>
 *     <filter-class>com.sharneng.webservlet.SpringFilterBinder</filter-class>
 *     <init-param>
 *         <param-name>WebFilterName</param-name>
 *         <param-value>loggingFilter</param-value>
 *     </init-param>
 * </filter>
 * }
 * </pre>
 * 
 * @author Kenneth Xu
 * 
 */
public class SpringFilterBinder extends AbstractFilterBinder {
    /** Name of the servlet init parameter that to specify a WebServlet bean in Spring context. */
    public static final String WEB_FILTER_NAME_PARAMETER = "WebFilterName";

    @Override
    protected Filter getWebFilter(FilterConfig config) throws ServletException {
        String webServletName = config.getInitParameter(WEB_FILTER_NAME_PARAMETER);
        final WebApplicationContext context = WebApplicationContextUtils.getWebApplicationContext(config
                .getServletContext());
        return (Filter) context.getBean(webServletName);
    }
}
