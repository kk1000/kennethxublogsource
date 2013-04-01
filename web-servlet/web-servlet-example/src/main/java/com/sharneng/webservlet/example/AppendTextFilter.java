package com.sharneng.webservlet.example;

import com.sharneng.webservlet.FilterBase;

import java.io.IOException;

import javax.servlet.ServletException;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

public class AppendTextFilter extends FilterBase<HttpServletRequest, HttpServletResponse> {

    private String messageToAppend;

    public void setMessageToAppend(String messageToAppend) {
        this.messageToAppend = messageToAppend;
    }

    public void afterService(HttpServletRequest request, HttpServletResponse response) throws IOException,
            ServletException {
        response.getOutputStream().println("<br/>Appended by filter: " + messageToAppend);
    }
}
