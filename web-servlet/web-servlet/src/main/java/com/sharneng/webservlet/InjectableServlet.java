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

import javax.servlet.Servlet;
import javax.servlet.ServletException;
import javax.servlet.ServletResponse;
import javax.servlet.http.HttpServlet;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

/**
 * Made functions of {@link HttpServlet} available through an interface.
 * <p>
 * Implementations of {@link InjectableServlet} should generally extends from {@link AbstractInjectableServlet} to avoid
 * the need of implementing every method of this interface.
 * <p>
 * The purpose of this interface and its abstract implementation {@link AbstractInjectableServlet} is to allow servlet
 * to participate in the dependency injection systems, and simplifies that task of writing the tests for servlets. For
 * example, {@link com.sharneng.webservlet.SpringBinder} is provided as a glue class to integrate with Spring Framework.
 * <p>
 * Please note that most of method names in this interface has preceding "do" removed from their counterparts in
 * {@link HttpServlet} class. This is done in purpose to avoid overriding those methods in
 * {@link AbstractInjectableServlet} class so that its parent's method {@link HttpServlet#doOptions} can generate the
 * correct option list.
 * 
 * @author Kenneth Xu
 * 
 */
public interface InjectableServlet extends Servlet {
    /**
     * Handles a DELETE request. The DELETE operation allows a client to remove a document or Web page from the server.
     * <p>
     * This method does not need to be either safe or idempotent. Operations requested through DELETE can have side
     * effects for which users can be held accountable. When using this method, it may be useful to save a copy of the
     * affected URL in temporary storage.
     * <p>
     * If the HTTP DELETE request is incorrectly formatted, {@code delete} returns an HTTP "Bad Request" message.
     * 
     * @param req
     *            the {@link HttpServletRequest} object that contains the request the client made of the servlet
     * @param resp
     *            the {@link HttpServletResponse} object that contains the response the servlet returns to the client
     * @throws ServletException
     *             if the request for the DELETE cannot be handled
     * @throws IOException
     *             if an input or output error occurs while the servlet is handling the DELETE request
     */
    void delete(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException;

    /**
     * Handles a GET request.
     * <p>
     * When implementing this method, read the request data, write the response headers, get the response's writer or
     * output stream object, and finally, write the response data. It's best to include content type and encoding. When
     * using a {@code PrintWriter} object to return the response, set the content type before accessing the
     * {@code PrintWriter} object.
     * <p>
     * The servlet container must write the headers before committing the response, because in HTTP the headers must be
     * sent before the response body.
     * <p>
     * Where possible, set the Content-Length header (with the {@link ServletResponse#setContentLength(int)} method), to
     * allow the servlet container to use a persistent connection to return its response to the client, improving
     * performance. The content length is automatically set if the entire response fits inside the response buffer.
     * <p>
     * When using HTTP 1.1 chunked encoding (which means that the response has a Transfer-Encoding header), do not set
     * the Content-Length header.
     * <p>
     * The GET method should be safe, that is, without any side effects for which users are held responsible. For
     * example, most form queries have no side effects. If a client request is intended to change stored data, the
     * request should use some other HTTP method.
     * <p>
     * The GET method should also be idempotent, meaning that it can be safely repeated. Sometimes making a method safe
     * also makes it idempotent. For example, repeating queries is both safe and idempotent, but buying a product online
     * or modifying data is neither safe nor idempotent.
     * <p>
     * If the request is incorrectly formatted, {@code get} returns an HTTP "Bad Request" message.
     * 
     * @param req
     *            the {@link HttpServletRequest} object that contains the request the client made of the servlet
     * @param resp
     *            the {@link HttpServletResponse} object that contains the response the servlet returns to the client
     * @throws ServletException
     *             if the request for the GET cannot be handled
     * @throws IOException
     *             if an input or output error occurs while the servlet is handling the GET request
     */
    void get(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException;

    /**
     * Handles an HTTP HEAD request. The client sends a HEAD request when it wants to see only the headers of a
     * response, such as Content-Type or Content-Length. The HTTP HEAD method counts the output bytes in the response to
     * set the Content-Length header accurately.
     * <p>
     * If you override this method, you can avoid computing the response body and just set the response headers directly
     * to improve performance. Make sure that the {@code head} method you write is both safe and idempotent (that is,
     * protects itself from being called multiple times for one HTTP HEAD request).
     * <p>
     * If the HTTP HEAD request is incorrectly formatted, {@code head} returns an HTTP "Bad Request" message.
     * 
     * @param req
     *            the {@link HttpServletRequest} object that contains the request the client made of the servlet
     * @param resp
     *            the {@link HttpServletResponse} object that contains the response the servlet returns to the client
     * @throws ServletException
     *             if the request for the HEAD cannot be handled
     * @throws IOException
     *             if an input or output error occurs while the servlet is handling the HEAD request
     */
    void head(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException;

    /**
     * Called by the server to allow a servlet to handle a OPTIONS request.
     * 
     * @param req
     *            the {@link HttpServletRequest} object that contains the request the client made of the servlet
     * @param resp
     *            the {@link HttpServletResponse} object that contains the response the servlet returns to the client
     * @throws ServletException
     *             if the request for the OPTIONS cannot be handled
     * @throws IOException
     *             if an input or output error occurs while the servlet is handling the OPTIONS request
     */
    void doOptions(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException;

    /**
     * Handles a POST request. The HTTP POST method allows the client to send data of unlimited length to the Web server
     * a single time and is useful when posting information such as credit card numbers.
     * <p>
     * When implementing this method, read the request data, write the response headers, get the response's writer or
     * output stream object, and finally, write the response data. It's best to include content type and encoding. When
     * using a {@code PrintWriter} object to return the response, set the content type before accessing the
     * {@code PrintWriter} object.
     * <p>
     * The servlet container must write the headers before committing the response, because in HTTP the headers must be
     * sent before the response body.
     * <p>
     * Where possible, set the Content-Length header (with the {@link ServletResponse#setContentLength(int)} method), to
     * allow the servlet container to use a persistent connection to return its response to the client, improving
     * performance. The content length is automatically set if the entire response fits inside the response buffer.
     * <p>
     * When using HTTP 1.1 chunked encoding (which means that the response has a Transfer-Encoding header), do not set
     * the Content-Length header.
     * <p>
     * This method does not need to be either safe or idempotent. Operations requested through POST can have side
     * effects for which the user can be held accountable, for example, updating stored data or buying items online.
     * <p>
     * If the HTTP POST request is incorrectly formatted, {@code post} returns an HTTP "Bad Request" message.
     * 
     * @param req
     *            the {@link HttpServletRequest} object that contains the request the client made of the servlet
     * @param resp
     *            the {@link HttpServletResponse} object that contains the response the servlet returns to the client
     * @throws ServletException
     *             if the request for the POST cannot be handled
     * @throws IOException
     *             if an input or output error occurs while the servlet is handling the POST request
     */
    void post(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException;

    /**
     * Handles a PUT request. The PUT operation allows a client to place a file on the server and is similar to sending
     * a file by FTP.
     * <p>
     * When implementing this method, leave intact any content headers sent with the request (including Content-Length,
     * Content-Type, Content-Transfer-Encoding, Content-Encoding, Content-Base, Content-Language, Content-Location,
     * Content-MD5, and Content-Range). If your method cannot handle a content header, it must issue an error message
     * (HTTP 501 - Not Implemented) and discard the request. For more information on HTTP 1.1, see RFC 2616 .
     * <p>
     * This method does not need to be either safe or idempotent. Operations that {@code put} performs can have side
     * effects for which the user can be held accountable. When using this method, it may be useful to save a copy of
     * the affected URL in temporary storage.
     * <p>
     * If the HTTP PUT request is incorrectly formatted, {@code put} returns an HTTP "Bad Request" message.
     * 
     * @param req
     *            the {@link HttpServletRequest} object that contains the request the client made of the servlet
     * @param resp
     *            the {@link HttpServletResponse} object that contains the response the servlet returns to the client
     * @throws ServletException
     *             if the request for the PUT cannot be handled
     * @throws IOException
     *             if an input or output error occurs while the servlet is handling the PUT request
     */
    void put(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException;

    /**
     * Handles a TRACE request. A TRACE returns the headers sent with the TRACE request to the client, so that they can
     * be used in debugging. There's no need to override this method.
     * 
     * @param req
     *            the {@link HttpServletRequest} object that contains the request the client made of the servlet
     * @param resp
     *            the {@link HttpServletResponse} object that contains the response the servlet returns to the client
     * @throws ServletException
     *             if the request for the TRACE cannot be handled
     * @throws IOException
     *             if an input or output error occurs while the servlet is handling the TRACE request
     */
    void trace(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException;

    /**
     * Returns the time the {@code HttpServletRequest} object was last modified, in milliseconds since midnight January
     * 1, 1970 GMT. If the time is unknown, this method returns a negative number (the default).
     * <p>
     * Servlets that support HTTP GET requests and can quickly determine their last modification time should override
     * this method. This makes browser and proxy caches work more effectively, reducing the load on server and network
     * resources.
     * 
     * @param req
     *            the {@code HttpServletRequest} object that is sent to the servlet
     * @return a {@code long} integer specifying the time the {@code HttpServletRequest} object was last modified, in
     *         milliseconds since midnight, January 1, 1970 GMT, or -1 if the time is not known
     */
    long getLastModified(HttpServletRequest req);

    /**
     * Handles standard HTTP requests.
     * 
     * @param req
     *            the {@link HttpServletRequest} object that contains the request the client made of the servlet
     * @param resp
     *            the {@link HttpServletResponse} object that contains the response the servlet returns to the client
     * @throws ServletException
     *             if the HTTP request cannot be handled
     * @throws IOException
     *             if an input or output error occurs while the servlet is handling the HTTP request
     */
    void service(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException;
}
