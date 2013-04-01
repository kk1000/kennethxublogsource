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

import static org.mockito.Mockito.verify;
import static org.mockito.Mockito.when;

import org.junit.Before;
import org.junit.Test;
import org.mockito.Mock;
import org.mockito.MockitoAnnotations;
import org.springframework.web.context.WebApplicationContext;

import javax.servlet.Filter;
import javax.servlet.FilterConfig;
import javax.servlet.ServletContext;

/**
 * Test {@link SpringFilterBinder}.
 * 
 * @author Kenneth Xu
 * 
 */
public class SpringFilterBindertTest {
    private static final String WebFilterName = "MyFilter";
    private AbstractFilterBinder sut;
    @Mock
    private ServletContext servletContext;
    @Mock
    private FilterConfig filterConfig;
    @Mock
    private WebApplicationContext springContext;
    @Mock
    private Filter webFilter;

    @Before
    public void setup() {
        MockitoAnnotations.initMocks(this);
        sut = new SpringFilterBinder();
        when(servletContext.getAttribute(WebApplicationContext.ROOT_WEB_APPLICATION_CONTEXT_ATTRIBUTE)).thenReturn(
                springContext);
        when(filterConfig.getInitParameter(SpringFilterBinder.WEB_FILTER_NAME_PARAMETER)).thenReturn(WebFilterName);
        when(filterConfig.getServletContext()).thenReturn(servletContext);
        when(springContext.getBean(WebFilterName)).thenReturn(webFilter);
    }

    @Test
    public void init_retrievesWebServletFromSpringContext() throws Exception {
        sut.init(filterConfig);
        verify(springContext).getBean(WebFilterName);
        verify(webFilter).init(filterConfig);
    }

}
