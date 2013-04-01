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

import javax.servlet.ServletRequest;
import javax.servlet.ServletResponse;

/**
 * Represents a servlet request/response interaction that has both the request and response object.
 * 
 * @param <Request>
 *            the type of the {@link ServletRequest}, for example {@link javax.servlet.HttpServletRequest}.
 * @param <Response>
 *            the type of the {@link ServletResponse}, for example {@link javax.servlet.HttpServletResponse}.
 * 
 * @author Kenneth Xu
 * 
 */
public interface ServletInteraction<Request extends ServletRequest, Response extends ServletResponse> {

    /**
     * The servlet request of the interaction.
     * 
     * @return the {@link ServletRequest} object in this interaction.
     */
    Request getServletRequest();

    /**
     * The servlet response of the interaction.
     * 
     * @return the {@link ServletResponse} object in this interaction.
     */
    Response getServletResponse();
}
