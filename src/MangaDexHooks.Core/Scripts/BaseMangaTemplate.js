const manga = JSON.parse(MangaString);
const chapter = JSON.parse(ChapterString);
const coverUrl = CoverUrlString;

const hook = {
    content: '',
    username: null,
    avatarUrl: null,
    tts: false,
    threadName: null,
    threadId: null,
    suppressEmbeds: false,
    embeds: []
};

class EmbedBuilder {

    constructor() {
        this.embed = {
            description: null,
            url: null,
            title: null,
            timestamp: null,
            color: null,
            image: null,
            thumbnail: null,
            author: null,
            footer: null,
            fields: []
        };
    }

    setTitle(title) {
        this.embed.title = title;
        return this;
    }

    setUrl(url) {
        this.embed.url = url;
        return this;
    }

    setDescription(description) {
        this.embed.description = description;
        return this;
    }

    setTimestamp(time) {
        this.embed.timestamp = time;
        return this;
    }

    setColor(r, g, b) {
        this.embed.color = ToColor(r, g, b);
        return this;
    }

    setImage(image) {
        this.embed.image = image;
        return this;
    }

    setThumbnail(url) {
        this.embed.thumbnail = url;
        return this;
    }

    setAuthor(name, url, iconUrl) {
        this.embed.author = { name, url, iconUrl };
        return this;
    }

    setFooter(text, iconUrl) {
        this.embed.footer = { text, iconUrl };
        return this;
    }

    addField(name, value, inline) {
        this.embed.fields.push({
            name,
            value,
            inline: !!inline
        });
        return this;
    }

    build() {
        return this.embed;
    }
}

class HookBuilder {

    setContent(value) {
        hook.content = value;
        return this;
    }

    setUser(value, avatar) {
        hook.username = value;
        hook.avatarUrl = avatar;
        return this;
    }

    setThread(name, id) {
        hook.threadName = name;
        hook.threadId = id;
        return this;
    }

    addEmbed(builder) {
        const bob = new EmbedBuilder();
        builder(bob);
        hook.embeds.push(bob.build());
        return this;
    }
}

builder = new HookBuilder();

function doScript() {
    /*INSERT SCRIPT HERE*/
}

doScript();

setOutput(JSON.stringify(hook));