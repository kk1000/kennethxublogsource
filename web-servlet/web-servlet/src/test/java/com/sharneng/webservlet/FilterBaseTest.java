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
import static org.junit.Assert.*;
import static org.mockito.Mockito.*;

import org.junit.Before;
import org.junit.Test;
import org.mockito.Mock;
import org.mockito.MockitoAnnotations;

import java.io.IOException;

import javax.servlet.FilterChain;
import javax.servlet.ServletException;
import javax.servlet.ServletRequest;
import javax.servlet.ServletResponse;

/**
 * Test {@link FilterBase}.
 * 
 * @author Kenneth Xu
 * 
 */
public class FilterBaseTest {

    private class MockFilter extends FilterBase {
        @Override
        public ServletInteraction beforeService(ServletRequest request, ServletResponse response) throws IOException,
                ServletException {
            beforeCallTime = System.currentTimeMillis();
            sleep(100);
            return super.beforeService(request, response);
        }

        @Override
        public void afterService(ServletRequest request, ServletResponse response) throws IOException, ServletException {
            afterCallTime = System.currentTimeMillis();
            sleep(100);
            reqAfter = request;
            respAfter = response;
            super.afterService(request, response);
        }
    }

    @Mock
    private ServletRequest req;
    @Mock
    private ServletRequest req2;
    @Mock
    private ServletResponse resp;
    @Mock
    private ServletResponse resp2;
    @Mock
    private FilterChain chain;

    private ServletRequest reqAfter;
    private ServletResponse respAfter;

    private long beforeCallTime;
    private long afterCallTime;
    private long chainCallTime;

    private FilterBase sut;

    private static void sleep(long millis) {
        try {
            Thread.sleep(millis);
        } catch (InterruptedException e) {
            sleep(millis);
        }
    }

    @Before
    public void setup() throws Exception {
        MockitoAnnotations.initMocks(this);
        sut = new MockFilter();
        when(req.getProtocol()).thenReturn("HTTP/1.1");
    }

    @Test
    public void doFilter_callesRightMethodsInRightOrder() throws Exception {
        chain = new FilterChain() {
            public void doFilter(ServletRequest request, ServletResponse response) throws IOException, ServletException {
                chainCallTime = System.currentTimeMillis();
                sleep(100);
            }
        };
        sut.doFilter(req, resp, chain);
        assertTrue("beforeService method must be called but wasn't", beforeCallTime > 0);
        assertTrue("chain.doFilter method must be called but wasn't", chainCallTime > 0);
        assertTrue("afterService method must be called but wasn't", afterCallTime > 0);
        assertTrue("beforeService method must be called before chain.doFilter method", beforeCallTime < chainCallTime);
        assertTrue("afterService method must be called after chain.doFilter method", afterCallTime > chainCallTime);
    }

    @Test
    public void doFilter_usesOriginalRequestResponse_byDefault() throws Exception {
        sut.doFilter(req, resp, chain);
        assertThat(reqAfter, is(req));
        assertThat(respAfter, is(resp));

        verify(chain).doFilter(req, resp);
    }

    @Test
    public void doFilter_usesReplacedRequestResponse_whenInteractionIsNotNull() throws Exception {
        sut = new MockFilter() {
            @Override
            public ServletInteraction beforeService(ServletRequest request, ServletResponse response)
                    throws IOException, ServletException {
                super.beforeService(request, response);
                return new SimpleInteraction(req2, resp2);
            }
        };
        sut.doFilter(req, resp, chain);
        assertThat(reqAfter, is(req2));
        assertThat(respAfter, is(resp2));

        verify(chain).doFilter(req2, resp2);
    }

    @Test
    public void init_noException() throws Exception {
        sut.init(null);
    }

    @Test
    public void destory_noException() throws Exception {
        sut.destroy();
    }
}
