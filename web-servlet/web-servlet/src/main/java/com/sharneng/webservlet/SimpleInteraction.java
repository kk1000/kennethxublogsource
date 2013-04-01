package com.sharneng.webservlet;

import javax.servlet.ServletRequest;
import javax.servlet.ServletResponse;

/**
 * A simple immutable implementation of {@link ServletInteraction}.
 * 
 * @author Kenneth Xu
 * 
 */
public class SimpleInteraction implements ServletInteraction {

    private final ServletRequest request;
    private final ServletResponse response;

    /**
     * Construct a new instance of {@code SimpleInteraction} with given request and response objects.
     * 
     * @param request
     *            the {@link ServletRequest} object that contains the client's request
     * @param response
     *            the {@link ServletResponse} object that contains the servlet's response
     */
    public SimpleInteraction(ServletRequest request, ServletResponse response) {
        if (request == null) throw new NullPointerException("request");
        if (response == null) throw new NullPointerException("response");
        this.request = request;
        this.response = response;
    }

    /**
     * {@inheritDoc}
     */
    public ServletRequest getServletRequest() {
        return request;
    }

    /**
     * {@inheritDoc}
     */
    public ServletResponse getServletResponse() {
        return response;
    }

}
