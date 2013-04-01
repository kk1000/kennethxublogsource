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

import static org.mockito.Mockito.*;

import org.junit.Before;
import org.junit.Test;
import org.mockito.Mock;
import org.mockito.MockitoAnnotations;

import javax.servlet.ServletConfig;
import javax.servlet.ServletContext;
import javax.servlet.ServletException;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

/**
 * Test {@link AbstractBinder}.
 * 
 * @author Kenneth Xu
 * 
 */
public class AbstractBinderTest {
    private AbstractBinder sut;
    @Mock
    private ServletContext servletContext;
    @Mock
    private ServletConfig servletConfig;
    @Mock
    private InjectableServlet webServlet;
    @Mock
    HttpServletRequest req;
    @Mock
    HttpServletResponse resp;

    @Before
    @SuppressWarnings("serial")
    public void setup() {
        MockitoAnnotations.initMocks(this);
        sut = new AbstractBinder() {
            @Override
            protected InjectableServlet getWebServlet(ServletConfig config) throws ServletException {
                return webServlet;
            }
        };
        when(servletConfig.getServletContext()).thenReturn(servletContext);
    }

    @Test
    public void init_retrievesWebServletAndInitializeIt() throws Exception {
        sut.init(servletConfig);
        verify(webServlet).init(servletConfig);
    }

    @Test
    public void service_delegatesToWebServlet() throws Exception {
        sut.init(servletConfig);
        sut.service(req, resp);
        verify(webServlet).service(req, resp);
    }

    @Test
    public void doGet_delegatesToWebServletGet() throws Exception {
        sut.init(servletConfig);
        sut.doGet(req, resp);
        verify(webServlet).get(req, resp);
    }

    @Test
    public void doPost_delegatesToWebServletPost() throws Exception {
        sut.init(servletConfig);
        sut.doPost(req, resp);
        verify(webServlet).post(req, resp);
    }

    @Test
    public void doPut_delegatesToWebServletPut() throws Exception {
        sut.init(servletConfig);
        sut.doPut(req, resp);
        verify(webServlet).put(req, resp);
    }

    @Test
    public void doDelete_delegatesToWebServletDelete() throws Exception {
        sut.init(servletConfig);
        sut.doDelete(req, resp);
        verify(webServlet).delete(req, resp);
    }

    @Test
    public void doHead_delegatesToWebServletHead() throws Exception {
        sut.init(servletConfig);
        sut.doHead(req, resp);
        verify(webServlet).head(req, resp);
    }

    @Test
    public void doTrace_delegatesToWebServletTrace() throws Exception {
        sut.init(servletConfig);
        sut.doTrace(req, resp);
        verify(webServlet).trace(req, resp);
    }

    @Test
    public void doOptions_delegatesToWebServletDoOptions() throws Exception {
        sut.init(servletConfig);
        sut.doOptions(req, resp);
        verify(webServlet).doOptions(req, resp);
    }

    @Test
    public void getLastModified_delegatesToWebServletGetLastModified() throws Exception {
        sut.init(servletConfig);
        sut.getLastModified(req);
        verify(webServlet).getLastModified(req);
    }

    @Test
    public void destroy_delegatesToWebServletDestroy() throws Exception {
        sut.init(servletConfig);
        sut.destroy();
        verify(webServlet).destroy();
    }
}
