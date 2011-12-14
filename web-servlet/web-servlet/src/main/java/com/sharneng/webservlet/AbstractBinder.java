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

import javax.servlet.ServletConfig;
import javax.servlet.ServletException;
import javax.servlet.http.HttpServlet;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

/**
 * Abstract servlet class to provides fundamental functionalities of binding to the {@link WebServlet}. All binder
 * servlet should inherit from this class and implement the only abstract method {@link #getWebServlet(ServletConfig)}.
 * The method should obtain an instance of {@link WebServlet} from the Dependency Injection framework. The binder
 * servlets are the classes that should be decleared in the web.xml file with certian parameter to help obtain the
 * specific instance of {@link WebServlet}.
 * 
 * @author Kenneth Xu
 * 
 */
@SuppressWarnings("serial")
public abstract class AbstractBinder extends HttpServlet {

    private WebServlet webServlet;

    /** {@inheritDoc} */
    @Override
    public void init(ServletConfig config) throws ServletException {
        super.init(config);
        webServlet = getWebServlet(config);
        webServlet.init(config);
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
    protected abstract WebServlet getWebServlet(ServletConfig config) throws ServletException;

    /** {@inheritDoc} */
    @Override
    protected final void doDelete(HttpServletRequest req, HttpServletResponse resp) throws ServletException,
            IOException {
        webServlet.delete(req, resp);
    }

    /** {@inheritDoc} */
    @Override
    protected final void doHead(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {
        webServlet.head(req, resp);
    }

    /** {@inheritDoc} */
    @Override
    protected final void doOptions(HttpServletRequest req, HttpServletResponse resp) throws ServletException,
            IOException {
        webServlet.doOptions(req, resp);
    }

    /** {@inheritDoc} */
    @Override
    protected final void doGet(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {
        webServlet.get(req, resp);
    }

    /** {@inheritDoc} */
    @Override
    protected final void doPost(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {
        webServlet.post(req, resp);
    }

    /** {@inheritDoc} */
    @Override
    protected final void doPut(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {
        webServlet.put(req, resp);
    }

    /** {@inheritDoc} */
    @Override
    protected final void doTrace(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {
        webServlet.trace(req, resp);
    }

    /** {@inheritDoc} */
    @Override
    protected final long getLastModified(HttpServletRequest req) {
        return webServlet.getLastModified(req);
    }

    /** {@inheritDoc} */
    @Override
    protected final void service(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {
        webServlet.service(req, resp);
    }

    /** {@inheritDoc} */
    @Override
    public final void destroy() {
        webServlet.destroy();
    }
}
