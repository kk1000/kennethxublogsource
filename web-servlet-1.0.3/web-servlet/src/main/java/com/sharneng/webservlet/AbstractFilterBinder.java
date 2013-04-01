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

import java.io.IOException;

import javax.servlet.Filter;
import javax.servlet.FilterChain;
import javax.servlet.FilterConfig;
import javax.servlet.ServletException;
import javax.servlet.ServletRequest;
import javax.servlet.ServletResponse;

/**
 * Abstract servlet filter to provides fundamental functionalities of binding to another filter instance that is managed
 * by dependency injection framework. All binder filter should inherit from this class and implement the only abstract
 * method {@link #getWebFilter(FilterConfig)}. The method should obtain the instance of filter from the Dependency
 * Injection framework. The binder filters are the classes that should be declared in the web.xml file with certain
 * parameter to help obtain the specific instance of filter that is managed by dependency framework.
 * 
 * @author Kenneth Xu
 * 
 */
public abstract class AbstractFilterBinder implements Filter {
    private Filter webFilter;

    /** {@inheritDoc} */
    public void init(FilterConfig filterConfig) throws ServletException {
        webFilter = getWebFilter(filterConfig);
        webFilter.init(filterConfig);
    }

    /** {@inheritDoc} */
    public void doFilter(ServletRequest request, ServletResponse response, FilterChain chain) throws IOException,
            ServletException {
        webFilter.doFilter(request, response, chain);
    }

    /** {@inheritDoc} */
    public final void destroy() {
        webFilter.destroy();
    }

    /**
     * Subclass to obtain an instance of {@code WebServlet} to delegate all service calls.
     * 
     * @param config
     *            the servlet configuration
     * @return A {@code WebServlet} instance
     * @throws ServletException
     *             if failed to obtain the {@code WebServlet}
     */
    protected abstract Filter getWebFilter(FilterConfig config) throws ServletException;
}
