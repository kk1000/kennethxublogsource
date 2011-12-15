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

import javax.servlet.ServletException;
import javax.servlet.http.HttpServlet;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

/**
 * Serve as the base class for implementations of {@link WebServlet}.
 * 
 * @author Kenneth Xu
 * 
 */
@SuppressWarnings("serial")
public abstract class AbstractWebServlet extends HttpServlet implements WebServlet {

    /** {@inheritDoc} */
    public void get(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {
        doGet(req, resp);
    }

    /** {@inheritDoc} */
    @Override
    public long getLastModified(HttpServletRequest req) {
        return super.getLastModified(req);
    }

    /** {@inheritDoc} */
    public void head(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {
        doHead(req, resp);
    }

    /** {@inheritDoc} */
    public void post(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {
        doPost(req, resp);
    }

    /** {@inheritDoc} */
    public void put(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {
        doPut(req, resp);
    }

    /** {@inheritDoc} */
    public void delete(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {
        doDelete(req, resp);
    }

    /** {@inheritDoc} */
    @Override
    public void doOptions(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {
        super.doOptions(req, resp);
    }

    /** {@inheritDoc} */
    public void trace(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {
        doTrace(req, resp);
    }

    /** {@inheritDoc} */
    @Override
    public void service(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {
        super.service(req, resp);
    }
}
