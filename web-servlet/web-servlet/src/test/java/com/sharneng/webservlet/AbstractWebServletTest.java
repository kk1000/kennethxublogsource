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

import static org.hamcrest.CoreMatchers.*;
import static org.junit.matchers.JUnitMatchers.*;
import static org.junit.Assert.*;
import static org.mockito.Mockito.*;

import org.junit.Before;
import org.junit.Test;
import org.mockito.Mock;
import org.mockito.MockitoAnnotations;

import java.io.IOException;
import java.util.Enumeration;

import javax.servlet.ServletException;
import javax.servlet.ServletOutputStream;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

/**
 * Test {@link AbstractWebServlet}
 * 
 * @author Kenneth Xu
 * 
 */
public class AbstractWebServletTest {

    @SuppressWarnings("serial")
    private static class WebServletMock extends AbstractWebServlet {
        @Override
        public void doGet(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {
        }
    }

    private AbstractWebServlet sut;
    @Mock
    private HttpServletRequest req;
    @Mock
    private HttpServletResponse resp;

    @SuppressWarnings("serial")
    @Before
    public void setup() {
        MockitoAnnotations.initMocks(this);
        sut = new AbstractWebServlet() {
        };
        when(req.getProtocol()).thenReturn("HTTP/1.1");
    }

    @Test
    public void service_callsDoGet_onGetRequest() throws Exception {
        when(req.getMethod()).thenReturn("GET");
        WebServletMock sut = spy(new WebServletMock());
        sut.service(req, resp);
        verify(sut).doGet(req, resp);
    }

    @Test
    public void getLastModified_returnsNegativeOne() {
        assertThat(sut.getLastModified(req), is(-1L));
    }

    @Test
    public void get_respondsWithMethodNotAllowed() throws Exception {
        sut.get(req, resp);
        verify(resp).sendError(eq(HttpServletResponse.SC_METHOD_NOT_ALLOWED), anyString());
    }

    @Test
    public void post_respondsWithMethodNotAllowed() throws Exception {
        sut.post(req, resp);
        verify(resp).sendError(eq(HttpServletResponse.SC_METHOD_NOT_ALLOWED), anyString());
    }

    @Test
    public void head_respondsWithMethodNotAllowed() throws Exception {
        sut.head(req, resp);
        verify(resp).sendError(eq(HttpServletResponse.SC_METHOD_NOT_ALLOWED), anyString());
    }

    @Test
    public void delete_respondsWithMethodNotAllowed() throws Exception {
        sut.delete(req, resp);
        verify(resp).sendError(eq(HttpServletResponse.SC_METHOD_NOT_ALLOWED), anyString());
    }

    @Test
    public void put_respondsWithMethodNotAllowed() throws Exception {
        sut.put(req, resp);
        verify(resp).sendError(eq(HttpServletResponse.SC_METHOD_NOT_ALLOWED), anyString());
    }

    @Test
    public void trace_echosBack() throws Exception {
        ServletOutputStream os = mock(ServletOutputStream.class);
        @SuppressWarnings("rawtypes")
        Enumeration headers = mock(Enumeration.class);
        when(req.getHeaderNames()).thenReturn(headers);
        when(resp.getOutputStream()).thenReturn(os);
        sut.trace(req, resp);
        verify(os).print("TRACE null HTTP/1.1\r\n");
    }

    @Test
    public void doOptions_setsAllowHeaderWithTraceAndOptions() throws Exception {
        sut.doOptions(req, resp);
        verify(resp).setHeader(eq("Allow"), argThat(both(containsString("TRACE")).and(containsString("OPTIONS"))));
    }

    @Test
    public void doOptions_setsAllowHeaderWithOverridenMethods() throws Exception {
        sut = new WebServletMock();
        sut.doOptions(req, resp);
        verify(resp).setHeader(eq("Allow"), argThat(both(containsString("GET")).and(not(containsString("POST")))));
    }
}
