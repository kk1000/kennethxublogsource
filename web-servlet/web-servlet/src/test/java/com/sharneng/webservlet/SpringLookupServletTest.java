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
import org.springframework.web.context.WebApplicationContext;

import javax.servlet.ServletConfig;
import javax.servlet.ServletContext;

public class SpringLookupServletTest {
    private static final String WebServletName = "MyWebSevlet";
    private AbstractBinder sut;
    @Mock
    private ServletContext servletContext;
    @Mock
    private ServletConfig servletConfig;
    @Mock
    private WebApplicationContext springContext;
    @Mock
    private WebServlet webServlet;

    @Before
    public void setup() {
        MockitoAnnotations.initMocks(this);
        sut = new SpringBinder();
        when(servletContext.getAttribute(WebApplicationContext.ROOT_WEB_APPLICATION_CONTEXT_ATTRIBUTE)).thenReturn(
                springContext);
        when(servletConfig.getInitParameter(SpringBinder.WEB_SERVLET_NAME_PARAMETER)).thenReturn(WebServletName);
        when(servletConfig.getServletContext()).thenReturn(servletContext);
        when(springContext.getBean(WebServletName)).thenReturn(webServlet);
    }

    @Test
    public void init_retrievesWebServletFromSpringContext() throws Exception {
        sut.init(servletConfig);
        verify(springContext).getBean(WebServletName);
        verify(webServlet).init(servletConfig);
    }

}
