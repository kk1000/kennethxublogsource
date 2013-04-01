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
 * Serves as a convenience based class for implementations of {@link Filter servlet filter}. Extends from this class so
 * that you don't have to implement every each methods of the {@link Filter} interface. In addition, instead having to
 * implement the {@link Filter#doFilter(ServletRequest, ServletResponse, FilterChain) method so you must remember to
 * call the {@link FilterChain#doFilter(ServletRequest, ServletResponse) chain.doFilter(request, response)}, you can
 * simply override {@link #beforeService(ServletRequest, ServletResponse)} and/or
 * {@link #afterService(ServletRequest, ServletResponse)} methods and leaving the work of calling
 * {@code chain.doFilter(...)} to {@link FilterBase}.
 * 
 * @param <Request>
 *            the type of the {@link ServletRequest}, for example {@link javax.servlet.HttpServletRequest}.
 * @param <Response>
 *            the type of the {@link ServletResponse}, for example {@link javax.servlet.HttpServletResponse}.
 * @author Kenneth Xu
 * 
 */
public class FilterBase<Request extends ServletRequest, Response extends ServletResponse> implements Filter {

    /**
     * {@inheritDoc}
     * <p>
     * This implementation does nothing. Subclass can override this if necessary. If you are developing filters that are
     * intended to be managed by the dependency injection framework, you probably want it to be configured there instead
     * of this method.
     */
    public void init(FilterConfig filterConfig) throws ServletException {
    }

    /**
     * {@inheritDoc}
     * <p>
     * This implementation does nothing. Subclass can override this if necessary. If you are developing filters that are
     * intended to be managed by the dependency injection framework, you probably want your filter's life cycle to be
     * managed there instead of this method.
     */
    public void destroy() {
    }

    /**
     * {@inheritDoc}
     * <p>
     * This implementation calls below methods in order.
     * <ol>
     * <li>{@link #beforeService(ServletRequest, ServletResponse) beforeService(request, response)}</li>
     * <li>{@link FilterChain#doFilter(ServletRequest, ServletResponse) chain.doFilter(request, response)}</li>
     * <li>{@link #afterService(ServletRequest, ServletResponse) afterService(request, response)}</li>
     * </ol>
     */
    public void doFilter(final ServletRequest request, final ServletResponse response, FilterChain chain)
            throws IOException, ServletException {
        @SuppressWarnings("unchecked")
        Request req = (Request) request;
        @SuppressWarnings("unchecked")
        Response resp = (Response) response;

        ServletInteraction<Request, Response> interaction = beforeService(req, resp);
        if (interaction != null) {
            req = interaction.getServletRequest();
            resp = interaction.getServletResponse();
        }

        chain.doFilter(req, resp);
        afterService(req, resp);
    }

    /**
     * Subclass can override this method to react to the servlet request and response before the final servlet or next
     * level of filter is called.
     * 
     * @param request
     *            the {@link ServletRequest} object that contains the client's request
     * @param response
     *            the {@link ServletResponse} object that contains the servlet's response
     * @return an {@link ServletInteraction} object that contains the replaced request and/or response objects, or null
     *         if neither request nor response object is replaced as a result.
     * @throws IOException
     *             if an input or output exception occurs
     * @throws ServletException
     *             if an exception occurs that interferes with the servlet's normal operation
     */
    public ServletInteraction<Request, Response> beforeService(Request request, Response response) throws IOException,
            ServletException {
        return null;
    }

    /**
     * Subclass can override this method to react to the servlet request and response after the final servlet or next
     * level of filter is called.
     * 
     * @param request
     *            the {@link ServletRequest} object that contains the client's request
     * @param response
     *            the {@link ServletResponse} object that contains the servlet's response
     * @throws IOException
     *             if an input or output exception occurs
     * @throws ServletException
     *             if an exception occurs that interferes with the servlet's normal operation
     */
    public void afterService(Request request, Response response) throws IOException, ServletException {
    }
}
