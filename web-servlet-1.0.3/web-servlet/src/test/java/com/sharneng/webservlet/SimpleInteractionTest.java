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

import org.junit.Before;
import org.junit.Rule;
import org.junit.Test;
import org.junit.rules.ExpectedException;
import org.mockito.Mock;
import org.mockito.MockitoAnnotations;

import javax.servlet.ServletRequest;
import javax.servlet.ServletResponse;

/**
 * Test {@link SimpleInteraction}.
 * 
 * @author Kenneth Xu
 * 
 */
public class SimpleInteractionTest {
    @Rule
    public ExpectedException exception = ExpectedException.none();

    @Mock
    private ServletRequest req;
    @Mock
    private ServletResponse resp;

    private SimpleInteraction<ServletRequest, ServletResponse> sut;

    @Before
    public void setup() throws Exception {
        MockitoAnnotations.initMocks(this);
    }

    @Test
    public void constructor_chokes_onNullRequest() throws Exception {
        exception.expect(NullPointerException.class);
        exception.expectMessage("request");

        new SimpleInteraction<ServletRequest, ServletResponse>(null, resp);
    }

    @Test
    public void constructor_chokes_onNullResponse() throws Exception {
        exception.expect(NullPointerException.class);
        exception.expectMessage("response");

        new SimpleInteraction<ServletRequest, ServletResponse>(req, null);
    }

    @Test
    public void getServletRequest_returnsValueSetInConstructor() throws Exception {
        sut = new SimpleInteraction<ServletRequest, ServletResponse>(req, resp);

        assertThat(sut.getServletRequest(), is(req));
    }

    @Test
    public void getServletResponse_returnsValueSetInConstructor() throws Exception {
        sut = new SimpleInteraction<ServletRequest, ServletResponse>(req, resp);

        assertThat(sut.getServletResponse(), is(resp));
    }
}
