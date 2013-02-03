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
package org.gm4java.engine.support;

import org.gm4java.engine.GMConnection;
import org.gm4java.engine.GMException;
import org.gm4java.engine.GMServiceException;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.Writer;

import javax.annotation.Nonnull;

/**
 * 
 * A implementation of {@link GMConnection} that serves as base of other implementation, and also used by
 * {@link SimpleGMService}.
 * 
 * @author Kenneth Xu
 * 
 */
class BasicGMConnection implements GMConnection {
    private static final int NORMAL_BUFFER_SIZE = 4096;
    private static final String EOL = System.getProperty("line.separator");
    private ReaderWriterProcess process;
    private StringBuffer sb = new StringBuffer();

    public BasicGMConnection(@Nonnull ReaderWriterProcess process) throws GMServiceException {
        if (process == null) throw new NullPointerException("process");
        this.process = process;
    }

    @Override
    public String execute(@Nonnull String command, String... arguments) throws GMException, GMServiceException {
        if (process == null) throw new GMServiceException("GMConnection is already closed.");
        sendCommand(command, arguments);
        return readResult();
    }

    @Override
    public void close() {
        if (process == null) return;
        process.destroy();
        process = null;
    }

    private void sendCommand(String command, String[] arguments) throws GMServiceException {
        Writer toGm = process.getWriter();
        try {
            toGm.write(command);
            for (String s : arguments) {
                final byte quote = '"';
                toGm.write(" ");
                toGm.write(quote);
                int start = 0, index = s.indexOf(quote);
                if (index < 0) {
                    toGm.write(s);
                } else {
                    do {
                        toGm.write(s, start, ++index - start);
                        toGm.write(quote);
                        start = index;
                        index = s.indexOf(quote, start);
                    } while (index >= 0);
                }
                toGm.write(quote);
            }
            toGm.write(EOL);
            toGm.flush();
        } catch (IOException e) {
            throw new GMServiceException(e.getMessage(), e);
        }
    }

    private String readResult() throws GMServiceException, GMException {
        String line;
        BufferedReader fromGm = process.getReader();
        sb.setLength(0);
        while ((line = readLine(fromGm)) != null) {
            if (line.equals(Constants.GM_PASS)) {
                return getGMOutput();
            }
            if (line.equals(Constants.GM_FAIL)) {
                throw new GMException(getGMOutput());
            }
            sb.append(line).append(EOL);
        }
        throw new GMServiceException("Input from GraphicsMagick was closed unexpectedly after receiving: "
                + getGMOutput());
    }

    private String getGMOutput() {
        String output = sb.toString();
        if (sb.length() > NORMAL_BUFFER_SIZE) {
            sb.setLength(0);
            sb.trimToSize();
        }
        return output;
    }

    private String readLine(BufferedReader reader) throws GMServiceException {
        try {
            return reader.readLine();
        } catch (IOException e) {
            throw new GMServiceException(e.getMessage(), e);
        }
    }

}
