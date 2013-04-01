package com.sharneng.webservlet;

import javax.servlet.ServletRequest;
import javax.servlet.ServletResponse;

/**
 * A simple immutable implementation of {@link ServletInteraction}.
 * 
 * @param <Request>
 *            the type of the {@link ServletRequest}, for example {@link javax.servlet.HttpServletRequest}.
 * 
 * @param <Response>
 *            the type of the {@link ServletResponse}, for example {@link javax.servlet.HttpServletResponse}.
 * 
 * @author Kenneth Xu
 * 
 */
public class SimpleInteraction<Request extends ServletRequest, Response extends ServletResponse> implements
        ServletInteraction<Request, Response> {

    private final Request request;
    private final Response response;

    /**
     * Construct a new instance of {@code SimpleInteraction} with given request and response objects.
     * 
     * @param request
     *            the {@link ServletRequest} object that contains the client's request
     * @param response
     *            the {@link ServletResponse} object that contains the servlet's response
     */
    public SimpleInteraction(Request request, Response response) {
        if (request == null) throw new NullPointerException("request");
        if (response == null) throw new NullPointerException("response");
        this.request = request;
        this.response = response;
    }

    /**
     * {@inheritDoc}
     */
    public Request getServletRequest() {
        return request;
    }

    /**
     * {@inheritDoc}
     */
    public Response getServletResponse() {
        return response;
    }

}
