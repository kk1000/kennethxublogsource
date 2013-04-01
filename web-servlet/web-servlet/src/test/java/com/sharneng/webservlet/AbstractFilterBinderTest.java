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

import javax.servlet.Filter;
import javax.servlet.FilterChain;
import javax.servlet.FilterConfig;
import javax.servlet.ServletContext;
import javax.servlet.ServletException;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

/**
 * Test {@link AbstractFilterBinder}.
 * 
 * @author Kenneth Xu
 * 
 */
public class AbstractFilterBinderTest {
    private AbstractFilterBinder sut;
    @Mock
    private ServletContext servletContext;
    @Mock
    private FilterConfig filterConfig;
    @Mock
    private Filter webFilter;
    @Mock
    HttpServletRequest req;
    @Mock
    HttpServletResponse resp;
    @Mock
    FilterChain chain;

    @Before
    public void setup() {
        MockitoAnnotations.initMocks(this);
        sut = new AbstractFilterBinder() {
            @Override
            protected Filter getWebFilter(FilterConfig config) throws ServletException {
                return webFilter;
            }
        };
        when(filterConfig.getServletContext()).thenReturn(servletContext);
    }

    @Test
    public void init_retrievesWebFilterAndInitializeIt() throws Exception {
        sut.init(filterConfig);
        verify(webFilter).init(filterConfig);
    }

    @Test
    public void doFilter_delegatesToWebFilter() throws Exception {
        sut.init(filterConfig);
        sut.doFilter(req, resp, chain);
        verify(webFilter).doFilter(req, resp, chain);
    }

    @Test
    public void destroy_delegatesToWebFilterDestroy() throws Exception {
        sut.init(filterConfig);
        sut.destroy();
        verify(webFilter).destroy();
    }
}
